// SolarSvr.cpp : ����Ӧ�ó������ڵ㡣
//

#include "stdafx.h"
#include "SawClient.h"
#include <math.h>
#include "kfft.h"


static LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    LONG_PTR DataPtr = GetWindowLongPtr(hWnd, GWLP_USERDATA);
    if (DataPtr != NULL)
    {
        SawClient *pClient = reinterpret_cast<SawClient *>(DataPtr);
        pClient->SawClientWndProc(hWnd, message, wParam, lParam);
    }
    return DefWindowProc(hWnd, message, wParam, lParam);
}

// �����ڡ������Ϣ�������
static INT_PTR CALLBACK AboutProc(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
    UNREFERENCED_PARAMETER(lParam);
    switch (message)
    {
    case WM_INITDIALOG:
        return (INT_PTR)TRUE;

    case WM_COMMAND:
        if (LOWORD(wParam) == IDOK || LOWORD(wParam) == IDCANCEL)
        {
            EndDialog(hDlg, LOWORD(wParam));
            return (INT_PTR)TRUE;
        }
        break;
    }
    return (INT_PTR)FALSE;
}

SawClient::SawClient(HINSTANCE hInstance)
    :hInst(hInstance)
    , hWnd(NULL)
    , classAtom(0)
    , m_pCfgParam(NULL)
{
	m_nIndex = 0;
    m_nPntPerScreen = 2048;
    m_pData = new float[m_nPntPerScreen];
    InitSawClient();
}

SawClient::~SawClient()
{
    delete[] m_pData;
}

void SawClient::Initialize(int nCmdShow)
{
    LoadString(hInst, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
    LoadString(hInst, IDC_SAWCLIENT, szWindowClass, MAX_LOADSTRING);

    RegisterWindowClass();
    CreateWindowInstance(nCmdShow);
}

void SawClient::CreateWindowInstance(int nCmdShow)
{
    hWnd = CreateWindow(szWindowClass, szTitle, WS_OVERLAPPEDWINDOW,
        CW_USEDEFAULT, 0, CW_USEDEFAULT, 0, NULL, NULL, hInst, NULL);

    if (!hWnd)
        return;
    SetWindowLongPtr(hWnd, GWLP_USERDATA, (LONG_PTR)this);
    ShowWindow(hWnd, nCmdShow);
    UpdateWindow(hWnd);
    SetScrollPos(hWnd, SB_HORZ, 0, TRUE);
    SetScrollPos(hWnd, SB_VERT, 0, TRUE);
	SetTimer(hWnd, 1, 500, TimeRefresh);
}

void SawClient::RegisterWindowClass()
{
    WNDCLASSEX wcex;

    wcex.cbSize = sizeof(WNDCLASSEX);

    wcex.style = CS_HREDRAW | CS_VREDRAW;
    wcex.lpfnWndProc = (WNDPROC)WndProc;
    wcex.cbClsExtra = 0;
    wcex.cbWndExtra = 0;
    wcex.hInstance = hInst;
    wcex.hIcon = LoadIcon(hInst, MAKEINTRESOURCE(IDI_SAWCLIENT));
    wcex.hCursor = LoadCursor(NULL, IDC_ARROW);
    wcex.hbrBackground = (HBRUSH)(COLOR_WINDOW + 1);  // no background drawing, please
    wcex.lpszMenuName = MAKEINTRESOURCE(IDC_SAWCLIENT);
    wcex.lpszClassName = szWindowClass;
    wcex.hIconSm = LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

    classAtom = RegisterClassEx(&wcex);
}

int SawClient::Run()
{
    MSG msg;
    HACCEL hAccelTable = LoadAccelerators(hInst, MAKEINTRESOURCE(IDC_SAWCLIENT));

    // ����Ϣѭ��: 
    while (GetMessage(&msg, NULL, 0, 0))
    {
        if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg))
        {
            TranslateMessage(&msg);
            DispatchMessage(&msg);
        }
    }
    return (int)msg.wParam;
}

LRESULT SawClient::SawClientWndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    bool bRet = false;
    int wmId, wmEvent;

    switch (message)
    {
    case WM_COMMAND:
        wmId = LOWORD(wParam);
        wmEvent = HIWORD(wParam);
        // �����˵�ѡ��: 
        switch (wmId)
        {
        case IDM_ABOUT:
            DialogBox(hInst, MAKEINTRESOURCE(IDD_ABOUTBOX), hWnd, AboutProc);
            break;
        case IDM_EXIT:
            DestroyWindow(hWnd);
            break;
        default:
            return DefWindowProc(hWnd, message, wParam, lParam);
        }
        break;
    case WM_MENUSELECT:
        break;
    case WM_PAINT:
        bRet = SawPaint();
        break;
    case WM_DESTROY:
        PostQuitMessage(0);
        break;
    case WM_LBUTTONDOWN:
        break;
    case WM_LBUTTONUP:
        break;
    case WM_MOUSEMOVE:
        break;
    case WM_HSCROLL:
    case WM_VSCROLL:
       break;
    case WM_SIZING:
        break;
    default:
        return DefWindowProc(hWnd, message, wParam, lParam);
    }

    return bRet ? 0 : 1;
}

bool SawClient::SawPaint()
{
    PAINTSTRUCT ps;
    HDC hdc = BeginPaint(hWnd, &ps);
    RECT rcPaint = ps.rcPaint;
    
    HDC hdcMemory = CreateCompatibleDC(hdc);

    int width = rcPaint.right - rcPaint.left;
    int height = rcPaint.bottom - rcPaint.top;
    HBITMAP hbitmap = CreateCompatibleBitmap(hdc, width, height);
    SelectObject(hdcMemory, hbitmap);
    //FillSolidRect
    SetBkColor(hdcMemory, RGB(255, 255, 255));
    ExtTextOut(hdcMemory, 0, 0, ETO_OPAQUE, &rcPaint, NULL, 0, NULL);    

        //��ͼ
	float fltScope = 250.0f;
	float nSampleFreq = 4000.0f;
	float nSignalFreq = 100.0f;	
    int yTime = 6 + (height - 12) / 4;
    int ySpec = height - 6;
    int x = 5;
    Rectangle(hdcMemory, 5, 5, width - 5, height - 5);
    double nXRadio = (width - 12)*1.0 / m_nPntPerScreen;
    double nYTimeRadio = (height - 12)*1.0 / fltScope / 4;

	HPEN penLine = CreatePen(PS_SOLID, 1, RGB(0, 0, 255));
    HPEN oldpen = (HPEN)SelectObject(hdcMemory, penLine);

    MoveToEx(hdcMemory, x, yTime, NULL);
    for (int i = 0; i < m_nPntPerScreen; i++)
	{
		int nIndex = i + m_nIndex;
        m_pData[i] = fltScope * sin(2 * M_PI * 1.1f * nIndex * nSignalFreq / nSampleFreq);
        LineTo(hdcMemory, x + i*nXRadio, yTime - m_pData[i] * nYTimeRadio);
	}
    m_nIndex += m_nPntPerScreen;

    float *pImage = new float[m_nPntPerScreen];
    float *pFftReal = new float[m_nPntPerScreen];
    float *pFftImage = new float[m_nPntPerScreen];
    for (int i = 0; i < m_nPntPerScreen; i++)
	{
		pImage[i] = 0.0f;
		pFftReal[i] = 0.0f;
		pFftImage[i] = 0.0f;
	}


    FourierTransform fftTrans;
    complex_f *pComplexData = new complex_f[m_nPntPerScreen];
    for (int i = 0; i < m_nPntPerScreen; i++)
    {
        pComplexData[i] = complex_f(m_pData[i], 0.0f);
    }
    fftTrans.FFT(pComplexData, m_nPntPerScreen);

    //������ilΪ1ʱm_pData���Ϊģ ��pImage���Ϊ���ǣ��Ƕ��ƣ���
    //FourierTransform::kfft(m_pData, pImage, m_nPntPerScreen, pFftReal, pFftImage, 0, 0);

    int nFFTCount = m_nPntPerScreen / 2;//FFT�ǶԳƵ�
    double dbMax = 0.0;
    int nPos = 0;
    for (int i = 0; i < nFFTCount; i++)
    {
        //��ֵ = ģ * 2 / ����;
        //m_pData[i] = sqrt(pFftReal[i] * pFftReal[i] + pFftImage[i] * pFftImage[i]) * 2 / m_nPntPerScreen;//��ֵ
         float fR = pComplexData[i].Real();
         float fI = pComplexData[i].Image();
         m_pData[i] =sqrt(fR*fR + fI*fI) * 2 / m_nPntPerScreen;//��ֵ
        if (m_pData[i] > dbMax)
        {
            dbMax = m_pData[i];
            nPos = i;
        }
    }
    //��1.1�ĳ���������ʵ��λ���в�ͬ
    int nCalcPos = nSignalFreq * m_nPntPerScreen / nSampleFreq + 1;
    //�ź�Ƶ����FFT�ϵ�����λ�ü��㹫ʽ
    //SignalFreq = SampleFreq*(Pos - 1)/m_nPntPerScreen

    double nXSpecRadio = (width- 12)*1.0 / nFFTCount;
    double nYSpecRadio = (height - 12)*1.0 / dbMax / 2;

	char cText[64];
    sprintf_s(cText, 64, "Value:%.5f, POS: %d, Calc Pos: %d", dbMax, nPos, nCalcPos);
    TextOutA(hdcMemory, x + 1, ySpec - (height - 12) / 2, cText, strlen(cText));
    MoveToEx(hdcMemory, x, ySpec, NULL);
    for (int i = 0; i < nFFTCount; i++)
	{
        LineTo(hdcMemory, x + i*nXSpecRadio, ySpec - m_pData[i] * nYSpecRadio);
	}

    SelectObject(hdcMemory, oldpen);
	DeleteObject(penLine);
    
    StretchBlt(hdc, 0, 0, width, height, hdcMemory, 0, 0, width, height, SRCCOPY);
    
    DeleteObject(hbitmap);
    DeleteObject(hdcMemory);
	EndPaint(hWnd, &ps);
    return true;
}

void SawClient::InitSawClient()
{
    m_pCfgParam = new class IniFile();
    m_pCfgParam->load("SawClient.cfg");
    string strPort = "9999";
    string strHost = "127.0.0.1";
    m_pCfgParam->getValue("Messstation", "STEUERUNG", "HOST", strHost);
    m_pCfgParam->getValue("Messstation", "STEUERUNG", "PORT", strPort);
    int nPort = atoi(strPort.c_str());
    
    saw.Init(strHost.c_str(), nPort);
}

void CALLBACK SawClient::TimeRefresh(HWND hwnd, UINT message, UINT iTimerID, DWORD dwTime)
{
	InvalidateRect(hwnd, NULL, FALSE);
}
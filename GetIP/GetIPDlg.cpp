// GetIPDlg.cpp : ʵ���ļ�
//

#include "stdafx.h"
#include "GetIP.h"
#include "GetIPDlg.h"
#include "afxdialogex.h"

#include <algorithm>   //toupper

#include <iphlpapi.h>
#pragma  comment(lib, "iphlpapi.lib")

#include <urlmon.h>
#pragma  comment(lib,"urlmon.lib")


#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// ����Ӧ�ó��򡰹��ڡ��˵���� CAboutDlg �Ի���

class CAboutDlg : public CDialogEx
{
public:
	CAboutDlg();

// �Ի�������
	enum { IDD = IDD_ABOUTBOX };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV ֧��

// ʵ��
protected:
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialogEx(CAboutDlg::IDD)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialogEx)
END_MESSAGE_MAP()


// CGetIPDlg �Ի���



CGetIPDlg::CGetIPDlg(CWnd* pParent /*=NULL*/)
	: CDialogEx(CGetIPDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CGetIPDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CGetIPDlg, CDialogEx)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
END_MESSAGE_MAP()


// CGetIPDlg ��Ϣ�������

BOOL CGetIPDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// ��������...���˵�����ӵ�ϵͳ�˵��С�

	// IDM_ABOUTBOX ������ϵͳ���Χ�ڡ�
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		BOOL bNameValid;
		CString strAboutMenu;
		bNameValid = strAboutMenu.LoadString(IDS_ABOUTBOX);
		ASSERT(bNameValid);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// ���ô˶Ի����ͼ�ꡣ  ��Ӧ�ó��������ڲ��ǶԻ���ʱ����ܽ��Զ�
	//  ִ�д˲���
	SetIcon(m_hIcon, TRUE);			// ���ô�ͼ��
	SetIcon(m_hIcon, FALSE);		// ����Сͼ��

    string strLocalIP = "";
    string strInternetIP = "";
    GetLocalIP(strLocalIP);
    GetInternetIP(strInternetIP);
    
    
#ifdef _UNICODE
    GetDlgItem(IDC_IPADDR_LOCAL)->SetWindowText(str2wstr(strLocalIP).c_str());
    GetDlgItem(IDC_IPADDR_INTERNET)->SetWindowText(str2wstr(strInternetIP).c_str());
#else
    GetDlgItem(IDC_IPADDR_LOCAL)->SetWindowText(strLocalIP.c_str());
    GetDlgItem(IDC_IPADDR_INTERNET)->SetWindowText(str2wstr(strInternetIP).c_str());
#endif
    

	return TRUE;  // ���ǽ��������õ��ؼ������򷵻� TRUE
}

void CGetIPDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialogEx::OnSysCommand(nID, lParam);
	}
}

// �����Ի��������С����ť������Ҫ����Ĵ���
//  �����Ƹ�ͼ�ꡣ  ����ʹ���ĵ�/��ͼģ�͵� MFC Ӧ�ó���
//  �⽫�ɿ���Զ���ɡ�

void CGetIPDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // ���ڻ��Ƶ��豸������

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// ʹͼ���ڹ����������о���
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// ����ͼ��
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialogEx::OnPaint();
	}
}

//���û��϶���С������ʱϵͳ���ô˺���ȡ�ù��
//��ʾ��
HCURSOR CGetIPDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}

bool CGetIPDlg::GetLocalIP(string& strIP)
{
    PIP_ADAPTER_INFO pIpAdapterInfo = new IP_ADAPTER_INFO();
    unsigned long stSize = sizeof(IP_ADAPTER_INFO);
    int nRel = GetAdaptersInfo(pIpAdapterInfo, &stSize);
    if (ERROR_BUFFER_OVERFLOW == nRel)
    {
        delete pIpAdapterInfo;
        pIpAdapterInfo = (PIP_ADAPTER_INFO)new BYTE[stSize];
        nRel = GetAdaptersInfo(pIpAdapterInfo, &stSize);
    }
    bool bfind = false;
    for (IP_ADAPTER_INFO *pAdapter = pIpAdapterInfo; pAdapter != NULL; pAdapter = pAdapter->Next)
    {
        string strDescription = pAdapter->Description;
        std::transform(strDescription.begin(), strDescription.end(), strDescription.begin(), toupper);
        //IF_TYPE_ETHERNET_CSMACD 6
        //IF_TYPE_IEEE80211 71
        if ( (pAdapter->Type == 6 && strDescription.find("PCI") != string::npos) || (pAdapter->Type == 71 && strDescription.find("WIRELESS") != string::npos) )
        {
            IP_ADDR_STRING* lstIP = &(pAdapter->IpAddressList);
            do
            {
                string strTemp = string(lstIP->IpAddress.String);
                if (strTemp != "0.0.0.0" && strTemp != "127.0.0.1")
                {
                    strIP = strTemp;
                    bfind = true;
                    break;
                }
                lstIP = lstIP->Next;
            } while (lstIP);
        }
        if (bfind)
            break;
    }
    delete pIpAdapterInfo;
    return true;
}

bool CGetIPDlg::GetInternetIP(string& strInternetIP)
{
    TCHAR Temp[MAX_PATH];
    int nLen = GetTempPath(MAX_PATH, Temp);
    TCHAR TempFile[MAX_PATH];
    GetTempFileName(Temp, // directory for tmp files
        TEXT("IP"),     // temp file name prefix 
        0,                // create unique name 
        TempFile);  // buffer for name 
    URLDownloadToFile(NULL, _T("https://api.ipify.org"), TempFile, 0, NULL);

    string strIPFile = wstr2str(TempFile);
    FILE *stream = NULL;
    if (fopen_s(&stream, strIPFile.c_str(), "r+t") == 0)
    {
        char cBuffer[16]; 
        memset(cBuffer, 0, 16);
        fread_s(cBuffer, 16, 1, 16, stream);
        fclose(stream);
        _unlink(strIPFile.c_str());
        strInternetIP = cBuffer;
    }

    return true;
}

wstring CGetIPDlg::str2wstr(const string& str)
{
    size_t convertedChars = 0;
    const char* source = str.c_str();
    int nLen = str.size();
    size_t charNum = nLen + 1;
    wchar_t* dest = new wchar_t[charNum];
    mbstowcs_s(&convertedChars, dest, charNum, source, nLen);
    wstring result = dest;
    delete[] dest;
    return result;
}

string CGetIPDlg::wstr2str(const wstring& wstr)
{
    size_t convertedChars = 0;
    const wchar_t* source = wstr.c_str();
    int nLen = wstr.size();
    size_t charNum = nLen + 1;
    char* dest = new char[charNum];
    wcstombs_s(&convertedChars, dest, charNum, source, nLen);
    string result = dest;
    delete[] dest;
    return result;
}
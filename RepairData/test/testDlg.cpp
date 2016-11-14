// testDlg.cpp : ʵ���ļ�
//

#include "stdafx.h"
#include "test.h"
#include "testDlg.h"
#include <math.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// ����Ӧ�ó��򡰹��ڡ��˵���� CAboutDlg �Ի���

class CAboutDlg : public CDialog
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

CAboutDlg::CAboutDlg() : CDialog(CAboutDlg::IDD)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialog)
END_MESSAGE_MAP()


// CtestDlg �Ի���




CtestDlg::CtestDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CtestDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
	m_pData = new double[512];
}

CtestDlg::~CtestDlg()
{
	delete [] m_pData;
}

void CtestDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CtestDlg, CDialog)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	//}}AFX_MSG_MAP
    ON_BN_CLICKED(IDC_BUTTON1, &CtestDlg::OnBnClickedButton1)
END_MESSAGE_MAP()


// CtestDlg ��Ϣ�������

BOOL CtestDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// ��������...���˵�����ӵ�ϵͳ�˵��С�

	// IDM_ABOUTBOX ������ϵͳ���Χ�ڡ�
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		CString strAboutMenu;
		strAboutMenu.LoadString(IDS_ABOUTBOX);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// ���ô˶Ի����ͼ�ꡣ��Ӧ�ó��������ڲ��ǶԻ���ʱ����ܽ��Զ�
	//  ִ�д˲���
	SetIcon(m_hIcon, TRUE);			// ���ô�ͼ��
	SetIcon(m_hIcon, FALSE);		// ����Сͼ��

	srand(time(NULL));
	int nScope = 100;
	double PI = 3.1415926;
	for(int i=0; i< 512; i++)
	{
		m_pData[i] = sin(2*PI*i/180)*nScope;
	}


	int nBlock = 128;
	int nCount = 512/nBlock;
	for(int i=0; i< nCount; i++)
	{
		double dbSum = 0.0;
		for(int j=i*nBlock; j<(i+1)*nBlock; j++)
		{
			dbSum += m_pData[j];
		}
		double dbAvg = dbSum/nBlock;

		for(int j=i*nBlock; j<(i+1)*nBlock; j++)
		{
			m_pData[j] -= dbAvg;
		}
	}


	return TRUE;  // ���ǽ��������õ��ؼ������򷵻� TRUE
}

void CtestDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialog::OnSysCommand(nID, lParam);
	}
}

// �����Ի��������С����ť������Ҫ����Ĵ���
//  �����Ƹ�ͼ�ꡣ����ʹ���ĵ�/��ͼģ�͵� MFC Ӧ�ó���
//  �⽫�ɿ���Զ���ɡ�

void CtestDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // ���ڻ��Ƶ��豸������

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// ʹͼ���ڹ��������о���
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
		CPaintDC dc(this); // ���ڻ��Ƶ��豸������

		CRect rcClient;
		GetClientRect(&rcClient);
		CPoint ptCenter = rcClient.CenterPoint();
		dc.MoveTo(rcClient.left,m_pData[0]+ptCenter.y);
		for(int i=1; i< 512; i++)
			dc.LineTo(rcClient.left+i,m_pData[i]+ptCenter.y);

		CDialog::OnPaint();
	}
}

//���û��϶���С������ʱϵͳ���ô˺���ȡ�ù����ʾ��
//
HCURSOR CtestDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}

void CtestDlg::RepairData(double *pData, int nCount)
{
	int nPos = 0;
	int nEnd =  0;
	double dbDelta1 = 0;
	while(nPos < nCount)
	{
		int nFind = 0;//���ٲ��Ҵ���
		if(nEnd > 1)
			nFind = nEnd-2;
		int nStart = GetFixPos(nFind,pData,nCount,dbDelta1);
		if(nStart == 0)
			nStart = nEnd;
		double dbDelta2 = 0;
		nEnd = GetFixPos(nStart,pData,nCount,dbDelta2);
		if(nEnd == 0)
		{
			//Ӧ�����һ��
			dbDelta1 = dbDelta2;
			nEnd = nCount;
		}
		for(int n=nStart; n<nEnd; n++)
			pData[n] += dbDelta1;
		nPos = nEnd;
	}

}

int CtestDlg::GetFixPos(int nStart,double *pData, int nCount,double& delta)
{
	double dbPrevPrev = pData[nStart];
	double dbPrev = pData[nStart+1];
	double dbCur = 0;
	double dbNext = 0;
	int nPos = 0;
	delta = 0;
	for(int i=nStart+2; i< nCount; i++)
	{
		dbCur = pData[i];
		dbNext = pData[i+1];
		if(dbPrev > dbPrevPrev && dbCur < dbPrev/*ͻ��*/&& dbNext > dbCur)//����
		{
			delta = dbPrev - dbCur + dbNext - dbCur;
			nPos = i;
			break;
		}
		else if(dbPrev < dbPrevPrev && dbCur > dbPrev/*ͻ��*/ && dbNext < dbCur)//�½�
		{
			delta = dbPrev - dbCur + dbNext - dbCur;
			nPos = i;
			break;
		}
		dbPrevPrev = dbPrev;
		dbPrev = dbCur;
	}
	if(delta == 0 && nStart >= 2)//Ӧ�����һ��
		delta = pData[nStart-1] - pData[nStart] + pData[nStart+1] - pData[nStart];
	return nPos;
}

void CtestDlg::OnBnClickedButton1()
{
    //����
    RepairData(m_pData,512);
    Invalidate(TRUE);
}

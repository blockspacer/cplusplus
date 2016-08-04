#include "mainwindow.h"
#include "ui_mainwindow.h"
#include <stdio.h>
#include "kfft.h"

MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindow)
{
    ui->setupUi(this);

    m_nIndex = 0;
    m_nPntPerScreen = 2048;
    m_pData = new float[m_nPntPerScreen];
    m_timer = new QTimer(this);
    connect(m_timer,SIGNAL(timeout()),this,SLOT(RefreshView()));//timeoutslot()Ϊ�Զ����
    m_timer->start(500);
}

MainWindow::~MainWindow()
{
    m_timer->stop();
    delete m_timer;
    delete ui;
    delete[] m_pData;
}

void MainWindow::paintEvent(QPaintEvent *)
{ 
    QImage image(size(),QImage::Format_ARGB32_Premultiplied);
    QPainter imagePainter(&image);
    imagePainter.initFrom(this);
    imagePainter.setRenderHint(QPainter::Antialiasing, true);
    imagePainter.eraseRect(rect());
    draw(&imagePainter);
    imagePainter.end();
    QPainter widgetPainter(this);
    widgetPainter.drawImage(0,0,image);
}

void MainWindow::draw(QPainter *painter)
{
    int width = rect().width();
    int height = rect().height();
     //��ͼ
    float fltScope = 250.0f;
    float nSampleFreq = 8000.0f;
    float nSignalFreq = 1000.0f;
    int yTime = 6 + (height - 12) / 4;
    int ySpec = height - 6;
    int x = 5;
    painter->drawRect(5, 5, width - 10, height - 10);
    double nXRadio = (width - 12)*1.0 / m_nPntPerScreen;
    double nYTimeRadio = (height - 12)*1.0 / fltScope / 4;

    painter->setPen(QPen(Qt::blue,1,Qt::SolidLine,Qt::RoundCap));

    QPointF *pPntLine = new QPointF[m_nPntPerScreen];
    for (int i = 0; i < m_nPntPerScreen; i++)
     {
        int nIndex = i + m_nIndex;
        m_pData[i] = fltScope * sin(2 * M_PI * 1.1f * nIndex * nSignalFreq / nSampleFreq);
        pPntLine[i] =QPointF(x + i*nXRadio,yTime - m_pData[i] * nYTimeRadio);
     }
    painter->drawPolyline(pPntLine,m_nPntPerScreen);
    delete[] pPntLine;

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
    for (int i = 0; i < m_nPntPerScreen; ++i)
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
    //sprintf_s(cText, 64, "Value:%.5f, POS: %d, Calc Pos: %d", dbMax, nPos, nCalcPos);
    sprintf(cText, "Value:%.5f, POS: %d, Calc Pos: %d", dbMax, nPos, nCalcPos);
    int nSize = painter->font().pointSize();
    painter->drawText(QPoint(x+nPos*nXSpecRadio,ySpec-(height - 12)/2+nSize),tr(cText));
    //QVector<QPoint> vecFftLine;
    //vecFftLine.push_back(QPoint(x,ySpec));
    QPointF *pFftLine = new QPointF[nFFTCount];
    for (int i = 0; i < nFFTCount; i++)
    {
        //vecFftLine.push_back(QPoint(x + i*nXSpecRadio,ySpec - m_pData[i] * nYSpecRadio));
        //vecFftLine.push_back(QPoint(x + i*nXSpecRadio,ySpec - m_pData[i] * nYSpecRadio));
        pFftLine[i] = QPointF(x + i*nXSpecRadio,ySpec - m_pData[i] * nYSpecRadio);
    }
    //painter->drawLines(vecFftLine);
    painter->drawPolyline(pFftLine,nFFTCount);
    delete[] pFftLine;
}


void MainWindow::RefreshView()
{
    repaint();
}

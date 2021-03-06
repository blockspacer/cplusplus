#pragma once

#include "resource.h"
#include "HalconCpp.h"
using namespace Halcon;

#define MAX_LOADSTRING 256

class EdgeClient
{
public:
    EdgeClient(HINSTANCE hInstance);
    ~EdgeClient();

public:
    LRESULT EdgeClientWndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam);
    void Initialize(int nCmdShow);
    int Run();

private:
    void InitEdgeClient();
    bool EdgePaint();
    void RegisterWindowClass();
    void CreateWindowInstance(int nCmdShow);
    HWND CreateImageWnd(HWND parent, RECT rcWnd);

private:
    // Windows interface stuff
    HINSTANCE hInst;
    HWND hWnd;
    ATOM classAtom;
    TCHAR szTitle[MAX_LOADSTRING];
    TCHAR szWindowClass[MAX_LOADSTRING];

private:
    void DealImage(const Hobject &Image,const HTuple& hWndHandle);
    volatile bool m_bCapture;
    HANDLE m_hThread;
    static UINT CameraAction(LPVOID pParam);

};
// stdafx.cpp : ֻ������׼�����ļ���Դ�ļ�
// SolarSvr.pch ����ΪԤ����ͷ
// stdafx.obj ������Ԥ����������Ϣ

#include "stdafx.h"

// TODO:  �� STDAFX.H ��
// �����κ�����ĸ���ͷ�ļ����������ڴ��ļ�������
void __cdecl Tprintf(const char *format, ...)
{
    char buf[4096], *p = buf;
    va_list args;
    va_start(args, format);
    p += _vsnprintf_s(p, sizeof(buf) - 2, 4094, format, args);
    va_end(args);
    while (p > buf  &&  isspace(p[-1]))//����4096�����ǰһ�ֽ�
        *--p = '\0';
    *p++ = '\r';
    *p++ = '\n';
    *p = '\0';
    OutputDebugString(buf);
}

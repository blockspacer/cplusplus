﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using ApduParam;

namespace FNTMain
{
    class LicenseCalc
    {
        //申请码处理逻辑：先用AuthKey将物理地址的信息加密，再用LicenseKey对其进行解密得到申请码
        //注册码处理逻辑：先用AuthKey将申请码解密，再用LicenseKey对其进行加密得到注册码
        public static readonly byte[] LicenseKey = { 0x6D, 0xC5, 0xB9, 0x49, 0xFC, 0xDD, 0x44, 0xCD, 0xB9, 0x35, 0x64, 0xA1, 0x83, 0x92, 0x83, 0xF8 };
        public static readonly byte[] AuthKey    = { 0x50, 0xA5, 0xAA, 0x9B, 0xD3, 0x89, 0x4C, 0xBB, 0x8F, 0x3F, 0x23, 0x14, 0xCD, 0x34, 0xDF, 0x84 };

        public static string GetSN()
        {
            try
            {
                XmlNode node = null;
                XmlDocument xml = new XmlDocument();
                string strXmlPath = Application.StartupPath + @"\reg.xml";
                xml.Load(strXmlPath);//按路径读xml文件
                XmlNode root = xml.DocumentElement;//指向根节点
                if (root.Name != "RegCode")
                    return "";
                node = root.SelectSingleNode("LicenseKey");
                return node.InnerText;
            }
            catch
            {
                return "";
            }
        }

        public static void SetSN(string strLicense)
        {
            XmlNode node = null;
            XmlDocument xml = new XmlDocument();
            string strXmlPath = Application.StartupPath + @"\reg.xml";
            XmlElement Root = xml.CreateElement("RegCode");
            xml.AppendChild(Root);
            XmlDeclaration xmldecl = xml.CreateXmlDeclaration("1.0", "utf-8", null);
            xml.InsertBefore(xmldecl, Root);

            node = xml.CreateNode(XmlNodeType.Element, "LicenseKey", "");
            node.InnerText = strLicense;
            Root.AppendChild(node);

            xml.Save(strXmlPath);
        }

        //计算申请码
        public static string CalcSrcCode(string strPhysicalCode)
        {
            byte[] parseCode = new byte[16];
            if (strPhysicalCode.Length != 32)
                return "";
            for (int i = 0; i < 16; i++)
            {
                parseCode[i] = Convert.ToByte(strPhysicalCode.Substring(i * 2, 2), 16);
            }
            byte[] TempData = DesCryptography.TripleEncryptData(parseCode, AuthKey);
            byte[] EncryptData = DesCryptography.TripleDecryptData(TempData, LicenseKey);
            return BitConverter.ToString(EncryptData).Replace("-", "");
        }

        //注册码验证
        public static bool LicenseVerify(string strSrcCode, string strLicenseCode)
        {
            if (strSrcCode.Length != 32 || strLicenseCode.Length != 32)
                return false;
            string strLicense = strLicenseCode.ToUpper();
            bool bOk = true;
            for (int i = 0; i < strLicense.Length; i++)
            {
                if (Char.IsDigit(strLicense[i]))
                {
                    continue;
                }
                else if (strLicense[i] >= 'A' && strLicense[i] <= 'F')
                {
                    continue;
                }
                else
                {
                    bOk = false;
                    break;
                }
            }
            if (!bOk)
                return false;
            byte[] parseCode = new byte[16];            
            for (int i = 0; i < 16; i++)
            {
                parseCode[i] = Convert.ToByte(strLicense.Substring(i * 2, 2), 16);
            }
            byte[] TempData = DesCryptography.TripleDecryptData(parseCode, LicenseKey);
            byte[] EncryptData = DesCryptography.TripleEncryptData(TempData, AuthKey);
            string strVerify = BitConverter.ToString(EncryptData).Replace("-","");
            return string.Equals(strSrcCode, strVerify);
        }
    }
}
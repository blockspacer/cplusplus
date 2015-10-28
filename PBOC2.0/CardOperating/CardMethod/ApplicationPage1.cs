﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using IFuncPlugin;
using SqlServerHelper;
using System.Data.SqlClient;
using ApduParam;
using ApduCtrl;
using ApduInterface;


namespace CardOperating
{
    public partial class CardApplicationTest
    {
        private static string m_strPIN = "999999";//由用户输入

        private bool m_bGray = false;   //卡已灰，不能扣款解锁        

        private int m_nBusinessSn;  //脱机交易序号
        private int m_nTerminalSn;  //终端交易序号


        public void Page1Init()
        {
            textPIN.Text = m_strPIN;
        }


        private void textPIN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Backspace)
                e.Handled = true;//不接受非数字值
        }

        private void textPIN_Validated(object sender, EventArgs e)
        {
            m_strPIN = textPIN.Text;
        }

        /// <summary>
        /// 圈存
        /// </summary>
        /// <param name="nMoneyLoad">圈存金额（单位：分）</param>
        /// <param name="TerminalId">圈存终端机编号</param>        
        private bool MoneyLoad(int nMoneyLoad, byte[] TerminalId)
        {
            if (!ReadUserCardAsn(1))
                return false;
            if (m_UserCardCtrl.VerifyUserPin(m_strPIN) != 1)
                return false;
            m_UserCardCtrl.UserCardLoad(m_ASN, TerminalId, nMoneyLoad, false);
            return true;
        }

        //圈存
        private void btnCardLoad_Click(object sender, EventArgs e)
        {
            decimal MoneyValue = 0;
            decimal.TryParse(textMoney.Text, System.Globalization.NumberStyles.AllowThousands, null, out MoneyValue);
            double dbMoneyLoad = decimal.ToDouble(MoneyValue);
            if (MoneyValue < 1 || m_nAppIndex != 1 || !OpenUserCard())
                return;

            byte[] TerminalId = new byte[6];
            if (PublicFunc.ByteDataEquals(TerminalId, m_TermialId))//未读到终端机编号，使用固定编号
                Buffer.BlockCopy(m_FixedTermialId, 0, TerminalId, 0, 6);
            else
                Buffer.BlockCopy(m_TermialId, 0, TerminalId, 0, 6);
            //圈存
            string strInfo = string.Format("对卡号{0}圈存{1}元", BitConverter.ToString(m_ASN), dbMoneyLoad.ToString("F2"));
            OnMessageOutput(new MsgOutEvent(0, strInfo));
            MoneyLoad((int)(dbMoneyLoad * 100.0), TerminalId);
            CloseUserCard();
        }

        private void btnBalance_Click(object sender, EventArgs e)
        {
            if (m_nAppIndex != 1 || !OpenUserCard() || !ReadUserCardAsn(1))
                return;
            string strInfo = string.Format("读取卡号{0}的余额，并检查是否灰锁。", BitConverter.ToString(m_ASN));
            OnMessageOutput(new MsgOutEvent(0, strInfo));
            if (m_UserCardCtrl.VerifyUserPin(m_strPIN) == 1)
            {
                m_bGray = false;
                //未灰锁时终端机编号输出为0
                int nCardStatus = 0;
                if (m_UserCardCtrl.UserCardGray(ref nCardStatus, m_TermialId, m_GTAC))
                {
                    if (nCardStatus == 2)
                    {
                        //当前TAC未读，需要清空后重读
                        m_UserCardCtrl.ClearTACUF();
                        nCardStatus = 0;
                        m_UserCardCtrl.UserCardGray(ref nCardStatus, m_TermialId, m_GTAC);
                        m_bGray = nCardStatus == 1 ? true : false;
                    }
                    else
                    {
                        m_bGray = nCardStatus == 1 ? true : false;
                    }
                    GrayFlag.CheckState = m_bGray ? CheckState.Checked : CheckState.Unchecked;
                    GrayFlag.Checked = m_bGray;
                }
                else
                {
                    GrayFlag.CheckState = CheckState.Indeterminate;
                    GrayFlag.Checked = false;
                }

                int nBalance = 0;
                if (m_UserCardCtrl.UserCardBalance(ref nBalance, BalanceType.Balance_ED))
                {
                    double dbBalance = (double)(nBalance / 100.0);
                    textBalance.Text = dbBalance.ToString("F2");
                }
                else
                {
                    textBalance.Text = "0.00";
                }
            }
            CloseUserCard();
        }

        //强制解灰
        private void btnUnlockGrayCard_Click(object sender, EventArgs e)
        {
            //未灰状态不可强制解灰
            if (m_nAppIndex != 1 || !m_bGray)
                return;
            if (!OpenUserCard() || !ReadUserCardAsn(1))
                return;
            if (m_UserCardCtrl.VerifyUserPin(m_strPIN) == 1)
            {
                const float BusinessMoney = 0.0F;//强制联机解灰 0 扣款
                byte[] TerminalId = new byte[6];
                if (PublicFunc.ByteDataEquals(TerminalId, m_TermialId))//未读到终端机编号，使用固定编号
                    Buffer.BlockCopy(m_FixedTermialId, 0, TerminalId, 0, 6);
                else
                    Buffer.BlockCopy(m_TermialId, 0, TerminalId, 0, 6);
                if (m_UserCardCtrl.UnLockGrayCard(m_ASN, TerminalId, (int)(BusinessMoney * 100.0), false,1))
                    m_bGray = false;
            }
            CloseUserCard();
        }

        private void btnLockCard_Click(object sender, EventArgs e)
        {
            if (m_bGray || m_nAppIndex != 1)
                return;
            if (!OpenUserCard() || !ReadUserCardAsn(1))
                return;
            if (m_UserCardCtrl.VerifyUserPin(m_strPIN) != 1)
                return;

           bool bSamSlot = SamSlot.Checked;//使用SAM卡槽
            if(!OpenSAMCard(bSamSlot))
                return;
            //灰锁初始化
            byte[] outData = new byte[15];
            m_UserCardCtrl.InitForGray(m_TermialId, outData);
            byte[] byteBalance = new byte[4];
            Buffer.BlockCopy(outData, 0, byteBalance, 0, 4);//ET余额
            byte[] OfflineSn = new byte[2];//ET脱机交易序号
            Buffer.BlockCopy(outData, 4, OfflineSn, 0, 2);
            byte keyVer = outData[9];
            byte keyFlag = outData[10];
            byte[] rand = new byte[4];
            Buffer.BlockCopy(outData, 11, rand, 0, 4);
            //灰锁
            const byte BusinessType = 0x91;//交易类型
            byte[] GrayLockData = new byte[19]; //从PSAM卡获得顺序为终端交易序号，终端随机数，BCD时间，MAC1

            if (!m_SamCardCtrl.SamAppSelect(bSamSlot))
                return;
            if (!m_SamCardCtrl.InitSamGrayLock(bSamSlot, m_TermialId, rand, OfflineSn, byteBalance, BusinessType, m_ASN, GrayLockData))
                return;
            byte[] GTAC = new byte[4];
            byte[] MAC2 = new byte[4];
            if (!m_UserCardCtrl.GrayLock(GrayLockData, GTAC, MAC2))
                return;
            if (!m_SamCardCtrl.VerifyMAC2(bSamSlot, MAC2,1))//验证MAC2
                return;
            m_nBusinessSn = (int)((OfflineSn[0] << 8) | OfflineSn[1]);
            m_nTerminalSn = (int)((GrayLockData[0] << 24) | (GrayLockData[1] << 16) | (GrayLockData[2] << 8) | GrayLockData[3]);
            if (!m_bContactCard)
                SamSlot.Enabled = false;
        }

        private byte[] GetDebitforUnlockData(bool bSamSlot)
        {
            byte[] DebitData = new byte[27];
            //计算GMAC
            const byte BusinessType = 0x93;//交易类型: 解0扣
            decimal Amount = 0;
            decimal.TryParse(textPurchase.Text, System.Globalization.NumberStyles.AllowThousands, null, out Amount);
            double dbAmount = decimal.ToDouble(Amount);
            if (dbAmount < 1)
                return null;
            int nMoneyAmount = (int)(dbAmount * 100.0); ////气票消费金额
            byte[] GMAC = new byte[4];
            if (!m_SamCardCtrl.CalcGMAC(bSamSlot, BusinessType, m_ASN, m_nBusinessSn, nMoneyAmount, GMAC))
                return null;
            byte[] byteMoney = BitConverter.GetBytes(nMoneyAmount); //气票消费金额
            DebitData[0] = byteMoney[3];
            DebitData[1] = byteMoney[2];
            DebitData[2] = byteMoney[1];
            DebitData[3] = byteMoney[0];
            m_nBusinessSn += 1;
            DebitData[4] = (byte)((m_nBusinessSn >> 8) & 0xFF);
            DebitData[5] = (byte)(m_nBusinessSn & 0xFF);
            Buffer.BlockCopy(m_TermialId, 0, DebitData, 6, 6);
            m_nTerminalSn += 1;
            DebitData[12] = (byte)((m_nTerminalSn >> 24) & 0xFF);//终端交易序号
            DebitData[13] = (byte)((m_nTerminalSn >> 16) & 0xFF);
            DebitData[14] = (byte)((m_nTerminalSn >> 8) & 0xFF);
            DebitData[15] = (byte)(m_nTerminalSn & 0xFF);
            byte[] SysTime = PublicFunc.GetBCDTime();
            Buffer.BlockCopy(SysTime, 0, DebitData, 16, 7);//BCD时间
            Buffer.BlockCopy(GMAC, 0, DebitData, 23, 4);//GMAC
            return DebitData;
        }

        private void btnUnlockCard_Click(object sender, EventArgs e)
        {
            if (m_bGray || m_UserCardCtrl == null)
                return;
            bool bSamSlot = SamSlot.Checked;
            byte[] UnlockData = null;
            if (m_SamCardCtrl != null)
            {
                UnlockData = GetDebitforUnlockData(bSamSlot);
                CloseSAMCard(bSamSlot);
            }
            if (!m_bContactCard)
                SamSlot.Enabled = true;

            if (UnlockData != null)
            {
                //解扣debit for unlock
                if (m_UserCardCtrl.DebitForUnlock(UnlockData))
                {
                    //清TACUF （即 读灰锁状态，但其中P1 == 0x01）
                    m_UserCardCtrl.ClearTACUF();
                }
            }
            CloseUserCard();
        }

        private void btnReadRecord_Click(object sender, EventArgs e)
        {
            if (m_nAppIndex != 1 || !OpenUserCard())
                return;
            if (!m_UserCardCtrl.SelectCardApp(1))
                return;
            if (m_UserCardCtrl.VerifyUserPin(m_strPIN) == 1)
            {
                List<CardRecord> lstRecord = m_UserCardCtrl.ReadRecord();
                if (lstRecord.Count > 0)
                {
                    FillListView(lstRecord);
                }
            }
            CloseUserCard();
        }


        private void FillListView(List<CardRecord> lstRecord)
        {
            RecordInCard.Items.Clear();
            foreach (CardRecord record in lstRecord)
            {
                ListViewItem item = new ListViewItem();
                item.Text = record.BusinessSn.ToString(); 
                double dbAmount = record.Amount / 100.0f;
                item.SubItems.Add(dbAmount.ToString("F2"));
                item.SubItems.Add(RecordType(record.BusinessType));
                item.SubItems.Add(record.TerminalID);
                item.SubItems.Add(record.BusinessTime);
                RecordInCard.Items.Add(item);
            }
        }

        private void btnUnload_Click(object sender, EventArgs e)
        {
            if (m_bGray || m_nAppIndex != 1)
                return;
            if (!OpenUserCard() || !ReadUserCardAsn(1))
                return;
            decimal MoneyUnLoad = 0;
            decimal.TryParse(textMoney.Text, System.Globalization.NumberStyles.AllowThousands, null, out MoneyUnLoad);
            double dbMoneyUnLoad = decimal.ToDouble(MoneyUnLoad);
            if (dbMoneyUnLoad < 1)
                return;
            //圈提
            string strInfo = string.Format("对卡号{0}圈提{1}元", BitConverter.ToString(m_ASN), dbMoneyUnLoad.ToString("F2"));
            OnMessageOutput(new MsgOutEvent(0, strInfo));
            if (m_UserCardCtrl.VerifyUserPin(m_strPIN) == 1)
            {
                byte[] TerminalId = new byte[6];
                if (PublicFunc.ByteDataEquals(TerminalId, m_TermialId))//未读到终端机编号，使用固定编号
                    Buffer.BlockCopy(m_FixedTermialId, 0, TerminalId, 0, 6);
                else
                    Buffer.BlockCopy(m_TermialId, 0, TerminalId, 0, 6);
                m_UserCardCtrl.UserCardUnLoad(m_ASN, TerminalId, (int)(dbMoneyUnLoad * 100.0), false);
            }
            CloseUserCard();
        }


    }
}
USE [FunnettStation]
GO

if exists (select * from sysobjects where id = object_id(N'PROC_PublishCpuCard') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure PROC_PublishCpuCard

/****** �û����ƿ������� ******/
SET ANSI_NULLS OFF
GO


SET QUOTED_IDENTIFIER ON
GO

/*************************************************************************************/

/*       Object:  Stored Procedure dbo.PROC_PublishCpuCard                      */

/*       ����ʱ�� 2015-04-03                                                                   */

/*       ��¼�ƿ�����������                                                                            */                                                                                                                 
/*************************************************************************************/

CREATE PROCEDURE PROC_PublishCpuCard(
	@CardId char(16),--����
	@CardType varchar(2),--������
	@ClientId int,--������λID
	@UseValidateDate datetime,--����Ч����
	@UseInvalidateDate datetime,--��ʧЧ����
	@Plate nvarchar(16),--����
	@SelfId varchar(50),--�Ա��
	@CertificatesType varchar(2), --֤������
	@PersonalId varchar(32), --֤��ID
	@DriverName nvarchar(50), --�ֿ�������
	@DriverTel varchar(32), --�ֿ��˵绰
	@VechileCategory nvarchar(8), --�����
	@SteelCylinderId varchar(32), --��ƿ���
	@CylinderTestDate datetime, --��ƿ��Ч��
	@Remark nvarchar(50), --��ע
	@R_OilTimesADay int, --ÿ���޼��ʹ���	
	@R_OilVolATime decimal(18, 2), --ÿ���޼�����
	@R_OilVolTotal decimal(18, 2), --ÿ���������
	@R_OilEndDate datetime, --���ͽ�ֹ����
	@R_Plate bit, --�޳���
	@R_Oil varchar(4), --����Ʒ
	@R_RFID bit, --�ޱ�ǩ
	@CylinderNum int, --��ƿ����
	@FactoryNum char(7), --��ƿ�������ұ��
	@CylinderVolume int, --��ƿ�ݻ�
	@BusDistance varchar(10) --����·��
	) With Encryption
 AS    
	declare @CpuKeyId int  --cpu����Կ���
	declare @OrgKeyId int
	declare @OrgKey char(32)
	declare @MasterKey char(32)
	
    declare @curTime datetime --ʱ��
    set @curTime = GETDATE()
    declare @KeyGuid uniqueidentifier
	set @KeyGuid=newid()
	--����ⲿ����������ִ�д洢����
	if (@@trancount<>0)
		return 1
	set xact_abort on                                         
	if(len(@CardId)<>16)
		return 2
	--�жϿ�������
	if exists(select * from Base_Card where CardNum=@CardId)
		return 3
begin
		--��ʼ����
		begin tran maintran
		insert into Base_Card values(@CardId,@CardType,@ClientId,0,@UseValidateDate,@UseInvalidateDate,
									@Plate,@SelfId,@CertificatesType,@PersonalId,@DriverName,@DriverTel,
									@VechileCategory,@SteelCylinderId,@CylinderTestDate,@Remark,
									0,0,0,0,0,@R_OilTimesADay,@R_OilVolATime,@R_OilVolTotal,@R_OilEndDate,
									@R_Plate,@R_Oil,@R_RFID,@CylinderNum,@FactoryNum,@CylinderVolume,@BusDistance,'0',@curTime,@KeyGuid,0);
		if(@@ERROR <> 0)
			begin
		    rollback tran maintran
		    return 4
		    end	
		select @OrgKeyId = OrgKeyId,@CpuKeyId = UseKeyID from Config_SysParams; --�������л�ȡ��ǰ��Ч����Կ��
		select @OrgKey = OrgKey from Key_OrgRoot where KeyId = @OrgKeyId and KeyType <> 1;
		select @MasterKey = MasterKey from Key_CpuCard where KeyId = @CpuKeyId;
		insert into Base_Card_Key select @KeyGuid,@OrgKey,@MasterKey,
					ApplicationIndex,ApplicationTendingKey,LoadKey,UnLoadKey,
					UnGrayKey,PINUnlockKey,PINResetKey from Key_CARD_ADF where RelatedKeyId = @CpuKeyId;
	commit tran miantran
	return 0
end
GO



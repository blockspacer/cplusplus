USE [FunnettStation]
GO

if exists (select * from sysobjects where id = object_id(N'PROC_PublishCpuCard') and type in (N'P', N'PC'))
drop procedure PROC_PublishCpuCard                                                                        
GO

/****** �û����ƿ������� ******/
SET ANSI_NULLS ON
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
	@BusDistance varchar(10), --����·��
	@UserKeyGuid uniqueidentifier,   --��Կ��Ϣ��GUID
	@RelatedMotherCard char(16) --�ӿ�������ĸ������
	) With Encryption
 AS    
	declare @SrcKeyGuid  uniqueidentifier
	
    declare @curTime datetime --ʱ��
    set @curTime = GETDATE()
	--����ⲿ����������ִ�д洢����
	if (@@trancount<>0)
		return 1
	set xact_abort on                                         
	if(len(@CardId)<>16)
		return 2
	--�жϿ�������
	if exists(select * from Base_Card where CardNum=@CardId)
		begin
		select @SrcKeyGuid = KeyGuid from Base_Card where CardNum=@CardId;
		delete from Base_Card where CardNum=@CardId;
		delete from Base_Card_Key where KeyGuid=@SrcKeyGuid;
		end
begin
		--��ʼ����
		begin tran maintran
		insert into Base_Card values(@CardId,@CardType,@ClientId,0,@RelatedMotherCard,@UseValidateDate,@UseInvalidateDate,
									@Plate,@SelfId,@CertificatesType,@PersonalId,@DriverName,@DriverTel,
									@VechileCategory,@SteelCylinderId,@CylinderTestDate,@Remark,
									0,0,0,0,0,@R_OilTimesADay,@R_OilVolATime,@R_OilVolTotal,@R_OilEndDate,
									@R_Plate,@R_Oil,@R_RFID,@CylinderNum,@FactoryNum,@CylinderVolume,@BusDistance,'0',@curTime,@UserKeyGuid,0);		
		if(@@ERROR <> 0)
			begin
		    rollback tran maintran
		    return 3
		    end	
		commit tran miantran
end
	return 0
GO



USE [FunnettStation]
GO

if exists (select * from sysobjects where id = object_id(N'PROC_RewriteCpuCard') and type in (N'P', N'PC'))
drop procedure PROC_RewriteCpuCard                                                                        
GO

/****** �û����ƿ������� ******/
SET ANSI_NULLS ON
GO


SET QUOTED_IDENTIFIER ON
GO

/*************************************************************************************/

/*       Object:  Stored Procedure dbo.PROC_RewriteCpuCard                      */

/*       ����ʱ�� 2015-04-03                                                                   */

/*       ��¼�ƿ�����������                                                                            */                                                                                                                 
/*************************************************************************************/

CREATE PROCEDURE PROC_RewriteCpuCard(
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
	@RelatedMotherCard char(16) --�ӿ�������ĸ������
	) With Encryption
 AS    	
    declare @curTime datetime --ʱ��
    declare @SrcClientId   int
    declare @SrcRelatedMotherCard char(16)
    declare @SrcPersonalId  varchar(32)
    declare @SrcDriverName nvarchar(50)
    declare @SrcDriverTel varchar(32)
    declare @SrcSteelCylinderId varchar(32)
    declare @SrcFactoryNum char(7)
    declare @LogContent nvarchar(1024)
    set @curTime = GETDATE()
	--����ⲿ����������ִ�д洢����
	if (@@trancount<>0)
		return 1
	set xact_abort on                                         
	if(len(@CardId)<>16)
		return 2
	--�жϿ�������
	if not exists(select * from Base_Card where CardNum=@CardId)
		return 3
begin
		select @SrcClientId=ClientId,@SrcRelatedMotherCard = RelatedMotherCard,@SrcPersonalId=PersonalId,@SrcDriverName=DriverName,@SrcDriverTel=DriverTel,@SrcSteelCylinderId= SteelCylinderId,@SrcFactoryNum=FactoryNum from Base_Card where CardNum=@CardId;
		--��ʼ����
		begin tran maintran
		update  Base_Card set ClientId = @ClientId,RelatedMotherCard=@RelatedMotherCard,UseValidateDate = @UseValidateDate, UseInvalidateDate = @UseInvalidateDate,
							Plate = @Plate,SelfId = @SelfId,PersonalId=@PersonalId,DriverName=@DriverName,DriverTel=@DriverTel,
							VechileCategory=@VechileCategory,SteelCylinderId=@SteelCylinderId,CylinderTestDate=@CylinderTestDate,Remark=@Remark,
							R_OilTimesADay=@R_OilTimesADay,R_OilVolATime=@R_OilVolATime,R_OilVolTotal=@R_OilVolTotal,R_OilEndDate=@R_OilEndDate,
							R_Plate=@R_Plate,R_Oil=@R_Oil,R_RFID=@R_RFID,CylinderNum=@CylinderNum,FactoryNum=@FactoryNum,CylinderVolume=@CylinderVolume,
							BusDistance=@BusDistance,OperateDateTime=@curTime where CardNum=@CardId;								
			if(@@ERROR <> 0)
			begin
		    rollback tran maintran
		    return 4
		    end		
		--�����޸Ŀ���Ϣ�ļ�¼
		set @LogContent = '�޸Ŀ���Ϣ��' + '������λ' + convert(varchar(10),@SrcClientId) + '->' + convert(varchar(10),@ClientId) + ';';
		set @LogContent	= 	@LogContent + '����ĸ��' + @SrcRelatedMotherCard + '->' + @RelatedMotherCard + ';';
		set @LogContent	= 	@LogContent + '֤����' + @SrcPersonalId + '->' + @PersonalId + ';';
		set @LogContent	= 	@LogContent + '�ֿ�������' + @SrcDriverName + '->' + @DriverName + ';';
		set @LogContent	= 	@LogContent + '�ֿ��˵绰' + @SrcDriverTel + '->' + @DriverTel + ';';
		set @LogContent	= 	@LogContent + '��ƿ���' + @SrcSteelCylinderId + '->' + @SteelCylinderId + ';';
		set @LogContent	= 	@LogContent + '��ƿ�������ұ��' + @SrcFactoryNum + '->' + @FactoryNum + ';';				             
		insert into Log_PublishCard values(@curTime,@LogContent,@ClientId,@CardId);
	commit tran miantran
	return 0
end
GO



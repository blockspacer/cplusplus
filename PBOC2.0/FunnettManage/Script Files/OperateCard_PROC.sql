USE [FunnettStation]
GO

if exists (select * from sysobjects where id = object_id(N'PROC_OperateCard') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure PROC_OperateCard

/****** ����ʧ���������� ******/
SET ANSI_NULLS ON
GO


SET QUOTED_IDENTIFIER ON
GO

/*************************************************************************************/

/*       Object:  Stored Procedure PROC_OperateCard                      */

/*       ����ʱ�� 2015-08-25                                                            */

/*       ����ʧ����������                                                                          */                                                                                                                 
/*************************************************************************************/

CREATE PROCEDURE PROC_OperateCard(
	@CardId char(16),--����
	@OperateName nvarchar(16),--��������
	@RelatedName nvarchar(50), --�ͻ�����
	@RelatedPersonalId varchar(32), --�ͻ�֤��ID
	@RelatedTel varchar(32), --�ͻ��绰
	@RePublishCardId char(16)--��������
	) With Encryption
 AS    
    declare @curTime datetime --ʱ��
	declare @OperateGuid  uniqueidentifier
    set @curTime = GETDATE()
	set @OperateGuid = NEWID()
	
	declare @CardType varchar(2),@PersonalId varchar(32),@DriverName nvarchar(50),@DriverTel varchar(32)
	declare @ClientId int, @CardBalance decimal(18,2), @AccountBalance decimal(18,2)
	declare @UseValidateDate datetime, @UseInvalidateDate datetime
	
	--����ⲿ����������ִ�д洢����
	if (@@trancount<>0)
		return 1
	set xact_abort on                                         
	if(len(@CardId)<>16)
		return 2
	if( (@OperateName = N'����') and (len(@RePublishCardId) = 16) ) --����
		begin
		if not exists(select * from Base_Card where CardNum=@CardId)
			return 3
		if not exists(select * from Base_Card where CardNum=@RePublishCardId)
			return 3
		end
begin
		--��ʼ����
		begin tran maintran
		select @CardType=CardType,@ClientId=ClientId,@UseValidateDate=UseValidateDate,@UseInvalidateDate=UseInvalidateDate,
				@PersonalId=PersonalId,@DriverName=DriverName,@DriverTel=DriverTel,@CardBalance=CardBalance,@AccountBalance=AccountBalance
				from Base_Card where CardNum = @CardId;
		insert into OperateCard_Record values(@OperateGuid,@CardId,@CardType,@ClientId,@UseValidateDate,@UseInvalidateDate,
									@PersonalId,@DriverName,@DriverTel,@CardBalance,@AccountBalance,@OperateName,
									@RePublishCardId,@RelatedName,@RelatedPersonalId,@RelatedTel,@curTime,0);		
		if(@@ERROR <> 0)
			begin
		    rollback tran maintran
		    return 4
		    end	
		commit tran miantran
end
	return 0
GO



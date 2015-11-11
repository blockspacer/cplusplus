USE [FunnettStation]
GO

if exists (select * from sysobjects where id = object_id(N'PROC_GetGrayRecord') and type in (N'P', N'PC'))
drop procedure PROC_GetGrayRecord                                                                        
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*************************************************************************************/

/*       Object:  Stored Procedure dbo.PROC_GetGrayRecord    */

/*       ����ʱ�� 2015-08-13                              */

/*       ��ȡ�ҿ��Ľ��׼�¼  */
                                                                                                                 
/*************************************************************************************/

CREATE PROCEDURE PROC_GetGrayRecord(
	@CardId char(16),    --����
	@PSAM_TID char(12),  --�ն˻����
	@GTAC    char(8),   --������GTAC
	@StationNo char(8) output, --վ����
	@GunNo int output,      --ǹ��
	@ConsumerTime datetime output, --����ʱ��
	@Price  decimal(18,2) output, --����
	@Gas    decimal(18,2) output, --����
	@Money  decimal(18,2) output, --���
	@ResidualAmount decimal(18,2) output --�������
	) With Encryption
 AS 
	declare @RecordCardId char(16)
	--����ⲿ����������ִ�д洢����
	if(@@trancount<>0)
		return 1
	set xact_abort on 
	--��ʼ����
	begin tran maintran	
	select @RecordCardId=FUserCardNo,@StationNo=FStationNO,@GunNo=FGunNo,@ConsumerTime=FTradeDateTime,
			@Price=FPrice,@Gas=FGas,@Money=FMoney,@ResidualAmount=FResidualAmount from SC_ConsumerDetail where FUserCardNo = @CardId and FPSAM_TID = @PSAM_TID and FTAC=@GTAC and FRecordType = '1';
	if(@@ERROR <> 0)
		begin
		rollback tran maintran
		return 1
		end	
	commit tran miantran
	if(len(@RecordCardId) <> 16)
	 return 1
	else
	 return 0
GO

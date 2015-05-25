USE [FunnettStation]
GO

if exists (select * from sysobjects where id = object_id(N'PROC_PublishPsamCard') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure PROC_PublishPsamCard

/****** PSAM���ƿ������� ******/
SET ANSI_NULLS OFF
GO


SET QUOTED_IDENTIFIER ON
GO

/*************************************************************************************/

/*       Object:  Stored Procedure dbo.PROC_PublishPsamCard                      */

/*       ����ʱ�� 2015-04-16                                                                   */

/*       ��¼�ƿ�����������                                                                            */                                                                                                                 
/*************************************************************************************/

CREATE PROCEDURE PROC_PublishPsamCard(
	@PsamCardId char(16),--PSAM����
	@TerminalId varchar(6),--�ն˻����
	@ClientId int,--������λID
	@UseValidateDate datetime,--����Ч����
	@UseInvalidateDate datetime,--��ʧЧ����
	@CompanyFrom varchar(8), --���з�
	@CompanyTo varchar(8), --���շ�
	@Remark nvarchar(50), --��ע
	@OrgKey char(32),    --��Ƭԭʼ��Կ
	@PsamMasterKey char(32) --������Կ
	) With Encryption
 AS    
    declare @curTime datetime --ʱ��
    set @curTime = GETDATE()
	--����ⲿ����������ִ�д洢����
	if (@@trancount<>0)
		return 1
	set xact_abort on                                         
	if(len(@PsamCardId)<>16 or @TerminalId <> 12 or @ClientId <=0)
		return 2
	--�жϿ�������
	if exists(select * from Psam_Card where PsamId=@PsamCardId)
	return 3
begin
		--��ʼ����
		begin tran maintran
		insert into Psam_Card values(@PsamCardId,@TerminalId,@ClientId,0,@UseValidateDate,@UseInvalidateDate,
									@CompanyFrom,@CompanyTo,@Remark,@curTime,@OrgKey,@PsamMasterKey,0);
		if(@@ERROR <> 0)
			begin
		    rollback tran maintran
		    return 4
		    end		 
	commit tran miantran
	return 0
end
GO



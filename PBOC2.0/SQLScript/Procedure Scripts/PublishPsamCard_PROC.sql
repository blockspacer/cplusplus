USE [FunnettStation]
GO

if exists (select * from sysobjects where id = object_id(N'PROC_PublishPsamCard') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure PROC_PublishPsamCard

/****** PSAM���ƿ������� ******/
SET ANSI_NULLS ON
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
	@TerminalId varchar(12),--�ն˻����
	@ClientId int,--������λID
	@UseValidateDate datetime,--����Ч����
	@UseInvalidateDate datetime,--��ʧЧ����
	@CompanyFrom varchar(16), --���з�
	@CompanyTo varchar(16), --���շ�
	@Remark nvarchar(50), --��ע
	@OrgKey char(32),    --��Ƭԭʼ��Կ
	@PsamMasterKey char(32) --������Կ
	) With Encryption
 AS    
    declare @SrcClientId   int
    declare @SrcOrgKey char(32)
    declare @SrcPsamMasterKey char(32)
    declare @LogContent nvarchar(1024)
    declare @curTime datetime --ʱ��
    set @curTime = GETDATE()
	--����ⲿ����������ִ�д洢����
	if (@@trancount<>0)
		return 1
	set xact_abort on                                         
	if(len(@PsamCardId)<>16 or len(@TerminalId) <> 12)
		return 2
	--�жϿ�������
	if exists(select * from Psam_Card where PsamId=@PsamCardId)
		begin
			select @SrcClientId=ClientId,@SrcOrgKey=OrgKey,@SrcPsamMasterKey= PsamMasterKey from Psam_Card where PsamId=@PsamCardId;
			--��ʼ����
			begin tran maintran
			update  Psam_Card set TerminalId = @TerminalId,ClientId=@ClientId,CardState = 1,UseValidateDate = @UseValidateDate, UseInvalidateDate = @UseInvalidateDate,
							IssueCode = @CompanyFrom,RecvCode = @CompanyTo,Remark=@Remark,OperateDateTime=@curTime,OrgKey=@OrgKey,PsamMasterKey=@PsamMasterKey;
			if(@@ERROR <> 0)
			begin
		    rollback tran maintran
		    return 4
		    end		
			--�����޸Ŀ���Ϣ�ļ�¼
			set @LogContent = '�޸Ŀ���Ϣ��' + '������λ' + convert(varchar(10),@SrcClientId) + '->' + convert(varchar(10),@ClientId) + ';';
			set @LogContent	= 	@LogContent + 'ԭʼ��Կ' + @SrcOrgKey + '->' + @OrgKey + ';';
			set @LogContent	= 	@LogContent + '������Կ' + @SrcPsamMasterKey + '->' + @PsamMasterKey + ';';
			set @LogContent	= 	@LogContent + '�ն˻����' + @TerminalId + ';';
			set @LogContent	= 	@LogContent + '���з�' + @CompanyFrom + ';';
			set @LogContent	= 	@LogContent + '���շ�' + @CompanyTo + ';';				             
		insert into Log_PublishCard values(@curTime,@LogContent,@ClientId,@PsamCardId);
	commit tran miantran
		end
	else
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
		end
	return 0
GO



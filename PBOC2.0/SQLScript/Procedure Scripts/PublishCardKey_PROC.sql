USE [FunnettStation]
GO

if exists (select * from sysobjects where id = object_id(N'PROC_PublishCardKey') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure PROC_PublishCardKey

/****** �û����ƿ������� ******/
SET ANSI_NULLS ON
GO


SET QUOTED_IDENTIFIER ON
GO

/*************************************************************************************/

/*       Object:  Stored Procedure dbo.PROC_PublishCardKey                      */

/*       ����ʱ�� 2015-04-03                                                                   */

/*       ��¼�ƿ�����������                                                                            */                                                                                                                 
/*************************************************************************************/

CREATE PROCEDURE PROC_PublishCardKey(
	@CardId char(16),--����
	@UserKeyGuid uniqueidentifier,   --��Կ��Ϣ��GUID
	@OrgKey char(32),		--��ʼ��Կ
	@MasterKey char(32),	--������Կ
	@ApplicationIndex int,	--Ӧ�ú�
	@AppTendingKey char(32),--Ӧ��ά����Կ
	@AppConsumerKey char(32), --������Կ
	@AppLoadKey char(32),	--Ȧ����Կ
	@AppUnLoadKey char(32),--Ȧ����Կ
	@AppUnGrayKeychar(32), --�����Կ	
	@AppPinUnlockKey char(32), --PIN������Կ
	@AppPinResetKey char(32), --PIN��װ��Կ
	) With Encryption
 AS    
	--����ⲿ����������ִ�д洢����
	if (@@trancount<>0)
		return 1
	set xact_abort on 
	if(len(@CardId)<>16)
		return 2                                        
	--�жϿ�������
	if not exists(select * from Base_Card where CardNum=@CardId)
		return 3;
begin
		--��ʼ����
		begin tran maintran
				insert into Base_Card_Key values(@UserKeyGuid,@OrgKey,@MasterKey,
					@ApplicationIndex,@AppTendingKey,@AppConsumerKey,@AppLoadKey,@AppUnLoadKey,
					@AppUnGrayKey,@AppPinUnlockKey,@AppPinResetKey);
		if(@@ERROR <> 0)
			begin
		    rollback tran maintran
		    return 4
		    end	
		commit tran miantran
end
	return 0
GO



USE [FunnettStation]
GO

if exists (select * from sysobjects where id = object_id(N'PROC_GetOrgKey') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure PROC_GetOrgKey

SET ANSI_NULLS OFF
GO


SET QUOTED_IDENTIFIER ON
GO

/*************************************************************************************/

/*       Object:  Stored Procedure dbo.PROC_GetOrgKey                      */

/*       ����ʱ�� 2015-05-14                                           */

/*       ��ȡ��ǰʹ�õĳ��̳�ʼ����Կ                       */
                                                                                                                 
/*************************************************************************************/

CREATE PROCEDURE PROC_GetOrgKey(
	@OrgKeyType int
	)With Encryption
 AS
    ---@OrgKeyType 0-CPU����1-PSAM��, 2-ͨ��
    declare @OrgId int  --ԭʼ��Կ���
	--����ⲿ����������ִ�д洢����
	if(@@trancount<>0)
		return 1
	set xact_abort on 
	--��ʼ����
	begin tran maintran
    if(@OrgKeyType <> 1)
		select @OrgId = OrgKeyId from Config_SysParams;
	else if(@OrgKeyType <> 0)
		select @OrgId = OrgPsamKeyId from Config_SysParams;
	select OrgKey from Key_OrgRoot where KeyId=@OrgId and (KeyType = @OrgKeyType or KeyType = 2);
	if(@@ERROR <> 0)
		begin
		rollback tran maintran
		return 1
		end	
	commit tran miantran
	return 0
GO



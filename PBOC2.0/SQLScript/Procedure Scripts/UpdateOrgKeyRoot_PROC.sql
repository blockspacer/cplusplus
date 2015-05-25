USE [FunnettStation]
GO

if exists (select * from sysobjects where id = object_id(N'PROC_UpdateOrgKeyRoot') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure PROC_UpdateOrgKeyRoot

/****** ���̳�ʼ����Կ��¼ ******/
SET ANSI_NULLS OFF
GO


SET QUOTED_IDENTIFIER ON
GO

/*************************************************************************************/

/*       Object:  Stored Procedure dbo.PROC_UpdateOrgKeyRoot                      */

/*       ����ʱ�� 2015-04-03                                            */

/*       ��¼���̳�ʼ����Կ                                             */
                                                                                                                 
/*************************************************************************************/

CREATE PROCEDURE PROC_UpdateOrgKeyRoot(
	@KeyId int,--��ԿID,���Ӽ�¼ʱ��Ч
	@OrgKey char(32),--��Կֵ
	@KeyType int,--��Կ����   ��0-cpu, 1-psam��
	@KeyDetail nvarchar(50), --��Կ����
	@KeyState bit,--��Կ״̬ ��0-��ʹ�ã�1-ʹ�ã�
	@DbState int, --�������ͣ�0-��������1-���£�2-���ӣ�3-ɾ����
	@AddKeyId int output --���Ӽ�¼ʱ�����ԿID
	) With Encryption
 AS
    declare @OrgKeyId int  --cpu��ԭʼ��Կ���
    declare @OrgPsamKeyId int  --psam��ԭʼ��Կ���
    declare @KeyIdDel int    --ɾ����¼ʱ����Config_SysParams
	--����ⲿ����������ִ�д洢����
	if( (@@trancount<>0) or (@DbState = 0))
		return 1
	set xact_abort on                                         
	if(@DbState<>3 and len(@OrgKey)<>32)
		return 2
	if(@DbState <> 2)
		begin
		if not exists(select * from Key_OrgRoot where KeyId=@KeyId)
			return 3
		end

	if(@DbState = 1) --���¼�¼		
		begin
		--��ʼ����
		begin tran maintran
		update Key_OrgRoot set OrgKey = @OrgKey,KeyType=@KeyType,InfoRemark=@KeyDetail where KeyId = @KeyId;
		if(@@ERROR <> 0)
			begin
		    rollback tran maintran
		    return 4
		    end	
		select @OrgKeyId = OrgKeyId, @OrgPsamKeyId = OrgPsamKeyId from Config_SysParams;
			if(@KeyState = 1)
				begin
				if(@KeyType = 0 and @OrgKeyId <> @KeyId)
					update Config_SysParams set OrgKeyId = @KeyId;
				else if(@KeyType = 1 and @OrgPsamKeyId <> @KeyId)
					update Config_SysParams set OrgPsamKeyId = @KeyId;
				end
			if(@@error<>0) 
				begin
				rollback tran maintran
				return 5
				end			      
		commit tran miantran
		return 0
	end -- //���¼�¼end
	else if(@DbState = 2)--��Ӽ�¼
		begin
		--��ʼ����
		begin tran maintran
		insert into Key_OrgRoot values(@OrgKey,@KeyType,@KeyDetail);
		set @AddKeyId = @@IDENTITY
		if(@@ERROR <> 0)
			begin
		    rollback tran maintran
		    return 4
		    end	
		select @OrgKeyId = OrgKeyId, @OrgPsamKeyId = OrgPsamKeyId from Config_SysParams;
		if(@KeyState = 1)
			begin
			if(@KeyType = 0 and @OrgKeyId <> @@IDENTITY)
				update Config_SysParams set OrgKeyId = @@IDENTITY;
			else if(@KeyType = 1 and @OrgPsamKeyId <> @@IDENTITY)
				update Config_SysParams set OrgPsamKeyId = @@IDENTITY;
			end
		if(@@error<>0) 
			 begin
			 rollback tran maintran
			 return 5
			 end			      
		commit tran miantran
		return 0
		end --��Ӽ�¼end
	else if(@DbState = 3)	--ɾ����¼	
		begin
		--��ʼ����
		begin tran maintran
		delete from Key_OrgRoot where KeyId = @KeyId;
		if(@@ERROR <> 0)
			begin
		    rollback tran maintran
		    return 4
		    end	
		select @OrgKeyId = OrgKeyId, @OrgPsamKeyId = OrgPsamKeyId from Config_SysParams;
		select @KeyIdDel = ISNULL(MAX(KeyId),1) from Key_OrgRoot;
		if(@KeyType = 0 and @OrgKeyId = @KeyId)
				update Config_SysParams set OrgKeyId = @KeyIdDel;
		else if(@KeyType = 1 and @OrgPsamKeyId = @KeyId)
				update Config_SysParams set OrgPsamKeyId = @KeyIdDel;
		if(@@error<>0) 
			 begin
			 rollback tran maintran
			 return 5
			 end			      
		commit tran miantran
		return 0
		end --ɾ����¼end
GO



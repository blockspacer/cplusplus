USE [FunnettStation]
GO

if exists (select * from sysobjects where id = object_id(N'PROC_UpdateAccount') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure PROC_UpdateAccount

/****** �û��˻� ******/
SET ANSI_NULLS ON
GO


SET QUOTED_IDENTIFIER ON
GO

/*************************************************************************************/

/*       Object:  Stored Procedure dbo.PROC_UpdateAccount    */

/*       ����ʱ�� 2015-05-21                              */

/*       ��¼�û��˻�����                                   */
                                                                                                                 
/*************************************************************************************/

CREATE PROCEDURE PROC_UpdateAccount(
	@UserId int,--�û�ID,���Ӽ�¼ʱ��Ч
	@UserName varchar(32),--�û���
	@Password varchar(64),--MD5���ܵ��û�����
	@Authority int,--Ȩ��
	@Status int,--״̬���Ƿ��½��
	@DbState int, --�������ͣ�0-��������1-���£�2-���ӣ�3-ɾ����	
	@AddUserId int output --���Ӽ�¼ʱ����û�ID
	) With Encryption
 AS    
	--����ⲿ����������ִ�д洢����
	if( (@@trancount<>0) or (@DbState = 0))
		return 1
	set xact_abort on  
	if(@DbState <> 2)
		begin
		if not exists(select * from UserDb where UserId=@UserId)
			return 2
		end

	if(@DbState = 1) --���¼�¼		
		begin
		--��ʼ����
		begin tran maintran
		update UserDb set Password = @Password,
							Authority=@Authority,
							Status = @Status where UserId = @UserId;
		if(@@ERROR <> 0)
			begin
		    rollback tran maintran
		    return 3
		    end	
		commit tran miantran
		return 0
	end -- //���¼�¼end
	else if(@DbState = 2)--��Ӽ�¼
		begin
		--��ʼ����
		begin tran maintran
		insert into UserDb values(@UserName,@Password,@Authority,@Status);
		set @AddUserId = @@IDENTITY
		if(@@ERROR <> 0)
			begin
		    rollback tran maintran
		    return 3
		    end	
		commit tran miantran
		return 0
		end --��Ӽ�¼end
	else if(@DbState = 3)	--ɾ����¼	
		begin
		--��ʼ����
		begin tran maintran
		delete from UserDb where UserId = @UserId;
		if(@@ERROR <> 0)
			begin
		    rollback tran maintran
		    return 3
		    end	
		commit tran miantran
		return 0
		end --ɾ����¼end
GO



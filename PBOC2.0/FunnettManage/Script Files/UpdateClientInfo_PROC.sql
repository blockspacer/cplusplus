USE [FunnettStation]
GO

if exists (select * from sysobjects where id = object_id(N'PROC_UpdateClientInfo') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure PROC_UpdateClientInfo

/****** ��λ��Ϣ��¼ ******/
SET ANSI_NULLS ON
GO


SET QUOTED_IDENTIFIER ON
GO

/*************************************************************************************/

/*       Object:  Stored Procedure dbo.PROC_UpdateClientInfo    */

/*       ����ʱ�� 2015-04-03                              */

/*       ���µ�λ�����Ϣ                    */
                                                                                                                 
/*************************************************************************************/

CREATE PROCEDURE PROC_UpdateClientInfo(
	@ClientId int,--��λID
	@ClientName nvarchar(50),--��λ����
	@ParentID int,--�ϼ���λID
	@ParentName nvarchar(50),  --�ϼ���λ����
	@Linkman nvarchar(12),  --��ϵ��
	@Telephone varchar(15), --��ϵ�绰
	@FaxNum varchar(50), --�������
	@Email varchar(50), --��������
	@ZipCode varchar(10), --�ʱ�
	@Address nvarchar(50), --��ַ
	@Bank nvarchar(50),  --��������
	@BankAccountNum varchar(25), --�����˻�
	@Remark nvarchar(50), --��ע
	@DbState int --�������ͣ�0-��������1-���£�2-���ӣ�3-ɾ����	
	) With Encryption
 AS 
    --����ⲿ����������ִ�д洢����
	if( (@@trancount<>0) or (@DbState = 0))
		return 1
	set xact_abort on   
	if(@ClientId <= 0 or len(@ClientName) <= 0)
		return 2		
	if(@DbState <> 2)
		begin
		if not exists(select * from Base_Client where ClientId=@ClientId)
			return 3
		end
	else
		begin
		if exists(select * from Base_Client where ClientId=@ClientId)--���Ӽ�¼��@ClientId�����Ѿ�����
			return 3
		end

	if(@DbState = 1) --���¼�¼		
		begin
		--��ʼ����
		begin tran maintran
		update Base_Client set ClientName = @ClientName,							
							Linkman=@Linkman,
							Telephone=@Telephone,
							FaxNum=@FaxNum,
							Email=@Email,
							Zipcode=@ZipCode,
							Address=@Address,
							Bank=@Bank,
							BankAccountNum=@BankAccountNum,
							Remark=@Remark where ClientId = @ClientId;
		if(@@ERROR <> 0)
			begin
		    rollback tran maintran
		    return 4
		    end	
  		update Base_Client set ParentName=@ClientName where ParentID = @ClientId;
		if(@@ERROR <> 0)
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
		insert into Base_Client values(@ClientId,@ClientName,@ParentID,@ParentName,@Linkman,@Telephone,@FaxNum,@Email,@ZipCode,@Address,@Bank,@BankAccountNum,@Remark,0,0);		
		if(@@ERROR <> 0)
			begin
		    rollback tran maintran
		    return 4
		    end	
		commit tran miantran
		return 0
		end --��Ӽ�¼end
	else if(@DbState = 3)	--ɾ����¼	
		begin
		--��ʼ����
		begin tran maintran
		delete from Base_Client where ClientId = @ClientId;		
		if(@@ERROR <> 0)
			begin
		    rollback tran maintran
		    return 4
		    end	
		delete from Base_Client where ParentID = @ClientId;
		if(@@error<>0) 
			 begin
			 rollback tran maintran
			 return 5
			 end			      
		commit tran miantran
		return 0
		end --ɾ����¼end
GO



USE [FunnettStation]
GO

if exists (select * from sysobjects where id = object_id(N'PROC_UpdatePsamKey') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure PROC_UpdatePsamKey

/****** PSAM����Կ��¼ ******/
SET ANSI_NULLS OFF
GO


SET QUOTED_IDENTIFIER ON
GO

/*************************************************************************************/

/*       Object:  Stored Procedure dbo.PROC_UpdatePsamKey    */

/*       ����ʱ�� 2015-04-03                              */

/*       ��¼PSAM����Կ                                   */
                                                                                                                 
/*************************************************************************************/

CREATE PROCEDURE PROC_UpdatePsamKey(
	@KeyId int,--��ԿID,���Ӽ�¼ʱ��Ч
	@MasterKey char(32),--������Կ
	@MasterTendingKey char(32),--��Ƭά����Կ
	@AppMasterKey char(32),--Ӧ��������Կ
	@AppTendingKey char(32),--Ӧ��ά����Կ
	@ConsumerMasterKey char(32),--��������Կ
	@GrayCardKey char(32),--������Կ
	@MacEncryptKey char(32),--MAC������Կ        
	@KeyDetail nvarchar(50), --��Կ����
	@KeyState bit,--��Կ״̬ ��0-��ʹ�ã�1-ʹ�ã�
	@DbState int, --�������ͣ�0-��������1-���£�2-���ӣ�3-ɾ����
	@AddKeyId int output --���Ӽ�¼ʱ�����ԿID
	) With Encryption
 AS    
    declare @PsamKeyId int  --psam����Կ���
    declare @KeyIdDel int    --ɾ����¼ʱ����Config_SysParams
	--����ⲿ����������ִ�д洢����
	if( (@@trancount<>0) or (@DbState = 0))
		return 1
	set xact_abort on  
	if(@DbState <> 3)                                       
		begin
		if(len(@MasterKey)<>32)
			return 2
		if(len(@MasterTendingKey)<>32)
			return 2
		if(len(@AppMasterKey)<>32)
			return 2
		if(len(@AppTendingKey)<>32)
			return 2
		if(len(@ConsumerMasterKey)<>32)
			return 2
		if(len(@GrayCardKey)<>32)
			return 2
		if(len(@MacEncryptKey)<>32)
			return 2
		end
	if(@DbState <> 2)
		begin
		if not exists(select * from Key_PsamCard where KeyId=@KeyId)
			return 3
		end

	if(@DbState = 1) --���¼�¼		
		begin
		--��ʼ����
		begin tran maintran
		update Key_PsamCard set MasterKey = @MasterKey,
							MasterTendingKey=@MasterTendingKey,
							ApplicatonMasterKey = @AppMasterKey,
							ApplicationTendingKey = @AppTendingKey,
							ConsumerMasterKey = @ConsumerMasterKey,
							GrayCardKey = @GrayCardKey,
							MacEncryptKey = @MacEncryptKey,
							InfoRemark = @KeyDetail where KeyId = @KeyId;
		if(@@ERROR <> 0)
			begin
		    rollback tran maintran
		    return 4
		    end	
		select @PsamKeyId = UsePsamKeyID from Config_SysParams;
		if(@KeyState = 1 and @PsamKeyId <> @KeyId)
			update Config_SysParams set UsePsamKeyID = @KeyId;			
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
		insert into Key_PsamCard values(@MasterKey,@MasterTendingKey,@AppMasterKey,@AppTendingKey,@ConsumerMasterKey,@GrayCardKey,@MacEncryptKey,@KeyDetail);
		set @AddKeyId = @@IDENTITY
		if(@@ERROR <> 0)
			begin
		    rollback tran maintran
		    return 4
		    end	
		select @PsamKeyId = UsePsamKeyID from Config_SysParams;
		if(@KeyState = 1 and @PsamKeyId <> @@IDENTITY)
			update Config_SysParams set UsePsamKeyID = @@IDENTITY;			
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
		delete from Key_PsamCard where KeyId = @KeyId;
		if(@@ERROR <> 0)
			begin
		    rollback tran maintran
		    return 4
		    end	
		select @PsamKeyId = UsePsamKeyID from Config_SysParams;
		if(@PsamKeyId = @KeyId)
			begin
			select @KeyIdDel = ISNULL(MAX(KeyId),1) from Key_PsamCard;
			update Config_SysParams set UsePsamKeyID = @KeyIdDel;
			end		
		if(@@error<>0) 
			 begin
			 rollback tran maintran
			 return 5
			 end			      
		commit tran miantran
		return 0
		end --ɾ����¼end
GO



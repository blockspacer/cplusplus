USE [FunnettStation]
GO

if exists (select * from sysobjects where id = object_id(N'PROC_UpdateCpuKey') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure PROC_UpdateCpuKey


if exists (select * from sysobjects where id = object_id(N'PROC_UpdateCpuAppKey') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure PROC_UpdateCpuAppKey

/****** CPU����Կ��¼ ******/
SET ANSI_NULLS ON
GO


SET QUOTED_IDENTIFIER ON
GO

/*************************************************************************************/

/*       Object:  Stored Procedure dbo.PROC_UpdateCpuKey    */

/*       ����ʱ�� 2015-04-03                              */

/*       ��¼CPU����Կ,�漰Key_CpuCard����Կ���Key_CARD_ADFӦ����Կ��                    */
                                                                                                                 
/*************************************************************************************/

CREATE PROCEDURE PROC_UpdateCpuKey(
	@KeyId int,--��ԿID,���Ӽ�¼ʱ��Ч
	@MasterKey char(32),--������Կ
	@MasterTendingKey char(32),--��Ƭά����Կ
	@InternalAuthKey char(32),  --��Ƭ�ڲ���֤��Կ
	@KeyDetail nvarchar(50),  --��Կ����
	@KeyState bit,--��Կ״̬ ��0-��ʹ�ã�1-ʹ�ã�
	@DbState int, --�������ͣ�0-��������1-���£�2-���ӣ�3-ɾ����
	@AddKeyId int output --���Ӽ�¼ʱ�����ԿID
	) With Encryption
 AS    
    declare @CpuKeyId int  --cpu����Կ���
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
		if(len(@InternalAuthKey)<>32)
			return 2
		end
	if(@DbState <> 2)
		begin
		if not exists(select * from Key_CpuCard where KeyId=@KeyId)
			return 3
		end

	if(@DbState = 1) --���¼�¼		
		begin
		--��ʼ����
		begin tran maintran
		update Key_CpuCard set MasterKey = @MasterKey,
							MasterTendingKey=@MasterTendingKey,
							InternalAuthKey = @InternalAuthKey,
							InfoRemark = @KeyDetail where KeyId = @KeyId;
		if(@@ERROR <> 0)
			begin
		    rollback tran maintran
		    return 4
		    end	
		select @CpuKeyId = UseKeyID from Config_SysParams;
		if(@KeyState = 1 and @CpuKeyId <> @KeyId)
			update Config_SysParams set UseKeyID = @KeyId;			
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
		insert into Key_CpuCard values(@MasterKey,@MasterTendingKey,@InternalAuthKey,@KeyDetail);
		set @AddKeyId = @@IDENTITY
		if(@@ERROR <> 0)
			begin
		    rollback tran maintran
		    return 4
		    end	
		select @CpuKeyId = UseKeyID from Config_SysParams;
		if(@KeyState = 1 and @CpuKeyId <> @@IDENTITY)
			update Config_SysParams set UseKeyID = @@IDENTITY;			
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
		delete from Key_CpuCard where KeyId = @KeyId;		
		if(@@ERROR <> 0)
			begin
		    rollback tran maintran
		    return 4
		    end	
		select @CpuKeyId = UseKeyID from Config_SysParams;
		if(@CpuKeyId = @KeyId)
			begin
			select @KeyIdDel = ISNULL(MAX(KeyId),1) from Key_CpuCard;			
			update Config_SysParams set UseKeyID = @KeyIdDel;
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


CREATE PROCEDURE PROC_UpdateCpuAppKey(
	@RelatedKeyId int,--��Ƭ��Կ���Ӧ����ԿID
	@AppIndex int,          --Ӧ�����
	@AppMasterKey char(32),--Ӧ��������Կ
	@AppTendingKey char(32),--Ӧ��ά����Կ
	@AppInternalAuthKey char(32), --Ӧ���ڲ���֤��Կ
	@PinResetKey char(32), --PIN������װ��Կ
	@PinUnlockKey char(32), --PIN������Կ
	@ConsumerMasterKey char(32),--��������Կ
	@LoadKey char(32), --Ȧ����Կ
	@TacMasterKey	char(32), --TAC��Կ
	@UnGrayKey char(32),  --���������Կ
	@UnLoadKey char(32), --Ȧ����Կ
	@OvertraftKey char(32),--�޸�͸֧�޶���Կ
	@DbState int --�������ͣ�0-��������1-���£�2-���ӣ�3-ɾ����	
	) With Encryption
 AS
	declare @CpuKeyId int  --cpu����Կ���
	declare @AppCount int  --Ӧ����
	--����ⲿ����������ִ�д洢����
	if( (@@trancount<>0) or (@DbState = 0))
		return 1
	set xact_abort on
	if(@DbState <> 3)                                         
		begin
		if(len(@AppMasterKey)<>32)
			return 2
		if(len(@AppTendingKey)<>32)
			return 2
		if(len(@AppInternalAuthKey)<>32)
			return 2
		if(len(@PinResetKey)<>32)
			return 2
		if(len(@PinUnlockKey)<>32)
			return 2
		if(len(@ConsumerMasterKey)<>32)	
			return 2
		if(len(@LoadKey)<>32)
			return 2
		if(len(@TacMasterKey)<>32)
			return 2
		if(len(@UnGrayKey)<>32)
			return 2	
		if(len(@UnLoadKey)<>32)
			return 2
		if(len(@OvertraftKey)<>32)
			return 2
		end
	if(@DbState <> 2)
		begin
		if not exists(select * from Key_CpuCard where KeyId=@RelatedKeyId)
			return 3
		end
	if(@DbState = 1) --���¼�¼		
		begin
		--��ʼ����
		begin tran maintran
		select @AppCount=COUNT(ADFKeyId) from Key_CARD_ADF where RelatedKeyId = @RelatedKeyId and ApplicationIndex = @AppIndex;
		if(@AppCount <> 1)
			return 3;
		update Key_CARD_ADF set ApplicatonMasterKey = @AppMasterKey,
								ApplicationTendingKey=@AppTendingKey,
								AppInternalAuthKey = @AppInternalAuthKey,
								PINResetKey = @PinResetKey,
								PINUnlockKey=@PinUnlockKey,
								ConsumerMasterKey=@ConsumerMasterKey,
								LoadKey=@LoadKey,
								TacMasterKey=@TacMasterKey,
								UnGrayKey=@UnGrayKey,
								UnLoadKey=@UnLoadKey,
								OverdraftKey=@OvertraftKey where RelatedKeyId = @RelatedKeyId and ApplicationIndex = @AppIndex;
		if(@@ERROR <> 0)
			begin
		    rollback tran maintran
		    return 4
		    end		
		commit tran miantran
		return 0
	end -- //���¼�¼end
	else if(@DbState = 2)--��Ӽ�¼
		begin
		--��ʼ����
		begin tran maintran
		select @AppCount=COUNT(ADFKeyId) from Key_CARD_ADF where RelatedKeyId = @RelatedKeyId and ApplicationIndex = @AppIndex;
		if(@AppCount <> 0)
			return 3;
		insert into Key_CARD_ADF values(@RelatedKeyId,@AppIndex,@AppMasterKey,@AppTendingKey,@AppInternalAuthKey,
										@PinResetKey,@PinUnlockKey,@ConsumerMasterKey,@LoadKey,@TacMasterKey,@UnGrayKey,@UnLoadKey,@OvertraftKey);
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
		select @AppCount= COUNT(ADFKeyId) from Key_CARD_ADF where RelatedKeyId = @RelatedKeyId and ApplicationIndex = @AppIndex;
		if(@AppCount <> 1)
			return 3;
		delete from Key_CARD_ADF where RelatedKeyId = @RelatedKeyId and ApplicationIndex = @AppIndex;
		if(@@ERROR <> 0)
			begin
		    rollback tran maintran
		    return 4
		    end	
		commit tran miantran
		return 0
		end --ɾ����¼end 
 GO

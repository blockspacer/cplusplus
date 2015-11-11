USE [FunnettStation]
GO

if exists (select * from sysobjects where id = object_id(N'PROC_UpdateCardState') and type in (N'P', N'PC'))
drop procedure PROC_UpdateCardState                                                                        
GO

/****** ����ʧ�ͽ�� ******/
SET ANSI_NULLS ON
GO


SET QUOTED_IDENTIFIER ON
GO

/*************************************************************************************/

/*       Object:  Stored Procedure PROC_UpdateCardState    */

/*       ����ʱ�� 2015-08-24                              */

/*       �����û���״̬����ʧ����ң�����ɾ������                                   */
                                                                                                                 
/*************************************************************************************/

CREATE PROCEDURE PROC_UpdateCardState(
	@CardId char(16),--����
	@CardState int,--״̬ 0-������1-��ʧ��2-�Ѳ�����3-���˿�
	@BlackCard bit--true������������false ��ɾ������
	) With Encryption
 AS    
	--����ⲿ����������ִ�д洢����
	if(@@trancount<>0)
		return 1
	set xact_abort on		
	--�жϿ�������
	if not exists(select * from Base_Card where CardNum=@CardId)
		return 2
begin
	declare @Today varchar(32)
	set @Today = Convert(varchar(32),GetDate(),120)	
		--��ʼ����
		begin tran maintran			
			update Base_Card set CardState = @CardState, OperateDateTime = GetDate() where CardNum=@CardId;
			if(@BlackCard = 1)
				begin	--����������					
				if not exists(select * from SC_BlackCard where FUserCardNo=@CardId)
					begin
					insert into SC_BlackAddCard values(@CardId,Left(@Today,10));
					insert into SC_BlackCard values(@CardId,Left(@Today,10));
					end				
				end
			else
				begin --��ɾ������
				if exists(select * from SC_BlackCard where FUserCardNo=@CardId)
					begin
					insert into SC_BlackDelCard values(@CardId,Left(@Today,10));				
					delete from SC_BlackCard where FUserCardNo = @CardId;
					end
				end
		if(@@ERROR <> 0)
			begin
		    rollback tran maintran
		    return 3
		    end	
		commit tran miantran
		return 0
end
GO



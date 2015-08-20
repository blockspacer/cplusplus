USE [FunnettStation]
GO

if exists (select * from sysobjects where id = object_id(N'PROC_GetPublishedCard') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure PROC_GetPublishedCard

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*************************************************************************************/

/*       Object:  Stored Procedure dbo.PROC_GetPublishedCard    */

/*       ����ʱ�� 2015-05-14                              */

/*       ��ȡ�Ѿ�������CPU����Ϣ  */
                                                                                                                 
/*************************************************************************************/

CREATE PROCEDURE PROC_GetPublishedCard(
	@CardNum char(16),    --����
	@ApplicationIndex int
	) With Encryption
 AS    
    declare @KeyGuid uniqueidentifier  --����ʱʹ����Կ��Ψһ���
	--����ⲿ����������ִ�д洢����
	if(@@trancount<>0)
		return 1
	set xact_abort on 
	--��ʼ����
	begin tran maintran
	select @KeyGuid = KeyGuid from Base_Card where CardNum = @CardNum;	
	select * from Base_Card inner join Base_Card_Key on Base_Card.KeyGuid = Base_Card_Key.KeyGuid where Base_Card.KeyGuid = @KeyGuid and Base_Card_Key.ApplicationIndex = @ApplicationIndex;
	if(@@ERROR <> 0)
		begin
		rollback tran maintran
		return 1
		end	
	commit tran miantran
	return 0
GO

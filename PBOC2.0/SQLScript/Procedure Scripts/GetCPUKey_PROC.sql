USE [FunnettStation]
GO

if exists (select * from sysobjects where id = object_id(N'PROC_GetCpuKey') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure PROC_GetCpuKey

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*************************************************************************************/

/*       Object:  Stored Procedure dbo.PROC_GetCpuKey    */

/*       ����ʱ�� 2015-05-14                              */

/*       ��ȡ��ǰʹ�õ�CPU����Կ,�漰Key_CpuCard����Կ���Key_CARD_ADFӦ����Կ��  */
                                                                                                                 
/*************************************************************************************/

CREATE PROCEDURE PROC_GetCpuKey(
	@ApplicationIndex int    --Ӧ������
	) With Encryption
 AS    
    declare @CpuKeyId int  --cpu����Կ���
	--����ⲿ����������ִ�д洢����
	if(@@trancount<>0)
		return 1
	set xact_abort on 
	--��ʼ����
	begin tran maintran
	select @CpuKeyId = UseKeyID from Config_SysParams;
	select * from Key_CpuCard inner join Key_CARD_ADF on Key_CpuCard.KeyId = Key_CARD_ADF.RelatedKeyId and Key_CpuCard.KeyId=@CpuKeyId and Key_CARD_ADF.ApplicationIndex = @ApplicationIndex;
	if(@@ERROR <> 0)
		begin
		rollback tran maintran
		return 1
		end	
	commit tran miantran
	return 0
GO

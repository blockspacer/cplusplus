USE [FunnettStation]
GO

if exists (select * from sysobjects where id = object_id(N'PROC_GetPsamKey') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure PROC_GetPsamKey

SET ANSI_NULLS OFF
GO


SET QUOTED_IDENTIFIER ON
GO

/*************************************************************************************/

/*       Object:  Stored Procedure dbo.PROC_GetPsamKey    */

/*       ����ʱ�� 2015-05-14                              */

/*       ��ȡ��ǰʹ�õ�Psam����Կ                */
                                                                                                                 
/*************************************************************************************/

CREATE PROCEDURE PROC_GetPsamKey With Encryption
 AS    
    declare @PsamKeyId int  --PSAM����Կ���
	--����ⲿ����������ִ�д洢����
	if(@@trancount<>0)
		return 1
	set xact_abort on 
	--��ʼ����
	begin tran maintran
	select @PsamKeyId = UsePsamKeyID from Config_SysParams;
	select * from Key_PsamCard where PsamId=@PsamKeyId;
	if(@@ERROR <> 0)
		begin
		rollback tran maintran
		return 1
		end	
	commit tran miantran
	return 0
GO



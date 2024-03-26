CREATE PROCEDURE KnowledgeItemReader
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM KnowledgeItem
    LEFT JOIN SysUser ON KnowledgeItem.OwnerID = SysUser.UserID
    WHERE KnowledgeItem.KnowledgeStatus != 'Deleted' OR KnowledgeItem.KnowledgeStatus IS NULL;
END;
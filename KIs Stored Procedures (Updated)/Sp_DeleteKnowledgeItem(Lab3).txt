CREATE PROCEDURE DeleteKnowledgeItem
    @KnowledgeID INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE KnowledgeItem
    SET KnowledgeStatus = 'Deleted'
    WHERE KnowledgeID = @KnowledgeID;
END;
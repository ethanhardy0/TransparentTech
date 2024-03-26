CREATE PROCEDURE RemoveKnowledgeItemCollab
    @KnowledgeID INT,
    @CollabID INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM KnowledgeCollab
    WHERE KnowledgeID = @KnowledgeID AND CollabID = @CollabID;
END;
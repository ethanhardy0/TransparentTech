CREATE PROCEDURE InsertKnowledgeItem
    @KnowledgeTitle NVARCHAR(255),
    @KnowledgeSubject NVARCHAR(255),
    @KnowledgeCategory NVARCHAR(255),
    @KnowledgeInformation NVARCHAR(MAX),
    @KnowledgePostDate DATETIME,
    @OwnerID INT,
    @Strengths NVARCHAR(MAX),
    @Weaknesses NVARCHAR(MAX),
    @Opportunities NVARCHAR(MAX),
    @Threats NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO KnowledgeItem (KnowledgeTitle, KnowledgeSubject, KnowledgeCategory, KnowledgeInformation, KnowledgePostDate, OwnerID, Strengths, Weaknesses, Opportunities, Threats)
    VALUES (@KnowledgeTitle, @KnowledgeSubject, @KnowledgeCategory, @KnowledgeInformation, @KnowledgePostDate, @OwnerID, @Strengths, @Weaknesses, @Opportunities, @Threats);

    SELECT CAST(scope_identity() AS INT);
END;
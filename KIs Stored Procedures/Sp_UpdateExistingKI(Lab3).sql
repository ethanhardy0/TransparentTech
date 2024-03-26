CREATE PROCEDURE UpdateExistingKI
    @KnowledgeID INT,
    @KnowledgeTitle NVARCHAR(255),
    @KnowledgeSubject NVARCHAR(255),
    @KnowledgeCategory NVARCHAR(255),
    @KnowledgeInformation NVARCHAR(MAX),
    @Strengths NVARCHAR(MAX),
    @Weaknesses NVARCHAR(MAX),
    @Opportunities NVARCHAR(MAX),
    @Threats NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE KnowledgeItem
    SET KnowledgeTitle = @KnowledgeTitle,
        KnowledgeSubject = @KnowledgeSubject,
        KnowledgeCategory = @KnowledgeCategory,
        KnowledgeInformation = @KnowledgeInformation,
        Strengths = @Strengths,
        Weaknesses = @Weaknesses,
        Opportunities = @Opportunities,
        Threats = @Threats
    WHERE KnowledgeID = @KnowledgeID;
END;
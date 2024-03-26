CREATE PROCEDURE UpdateExistingUser
    @UserID INT,
    @Username NVARCHAR(50),
    @Email NVARCHAR(100),
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @Street NVARCHAR(100),
    @City NVARCHAR(50),
    @State NVARCHAR(50),
    @Zip NVARCHAR(20),
    @Phone NVARCHAR(20),
    @UserType NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE SysUser
    SET Username = @Username,
        Email = @Email,
        FirstName = @FirstName,
        LastName = @LastName,
        Street = @Street,
        City = @City,
        State = @State,
        Zip = @Zip,
        Phone = @Phone,
        UserType = @UserType
    WHERE UserID = @UserID;
END;
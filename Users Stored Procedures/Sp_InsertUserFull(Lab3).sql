CREATE PROCEDURE InsertUserFull
    @Username NVARCHAR(50),
    @Email NVARCHAR(100),
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @Phone NVARCHAR(20),
    @Street NVARCHAR(100),
    @City NVARCHAR(50),
    @State NVARCHAR(50),
    @Zip NVARCHAR(20),
    @UserType NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO SysUser (Username, Email, FirstName, LastName, Phone, Street, City, State, Zip, UserType)
    VALUES (@Username, @Email, @FirstName, @LastName, @Phone, @Street, @City, @State, @Zip, @UserType);

    SELECT CAST(scope_identity() AS INT);
END;
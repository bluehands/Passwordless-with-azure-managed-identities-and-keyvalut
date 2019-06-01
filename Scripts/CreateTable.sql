CREATE TABLE [dbo].[Persons](
         [PersonId] [int] IDENTITY(1,1),
         [CreditCard] [nvarchar](50) NOT NULL,
         [Name] [nvarchar](50) NULL,
         [Amount] [int] NOT NULL
         PRIMARY KEY CLUSTERED ([PersonId] ASC) ON [PRIMARY] );
         GO
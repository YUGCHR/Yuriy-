/****** Скрипт для команды SelectTopNRows из среды SSMS  ******/
SELECT TOP (2000) [ID]
      ,[ID_Language]
      ,[ID_Chapter]
      ,[ID_Paragraph]
      ,[Sentence]
      ,[Sentence_name]
  FROM [TextSplitSentences].[dbo].[Sentences]

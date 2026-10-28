/* Проверка БД DEMO2023 после импорта */
USE [DEMO2023]
GO
-- 1. Количество строк (ожидается: Role 4, Gender 2, City 1117, Country 251, Course 5, User 61, Event 20, Activity 30, Mark 122)
SELECT 'Role' T, COUNT(*) N FROM [Role] UNION ALL SELECT 'Gender', COUNT(*) FROM Gender UNION ALL
SELECT 'City', COUNT(*) FROM City UNION ALL SELECT 'Country', COUNT(*) FROM Country UNION ALL
SELECT 'Course', COUNT(*) FROM Course UNION ALL SELECT 'User', COUNT(*) FROM [User] UNION ALL
SELECT 'Event', COUNT(*) FROM [Event] UNION ALL SELECT 'Activity', COUNT(*) FROM Activity UNION ALL
SELECT 'Mark', COUNT(*) FROM Mark;
-- 2. Пользователи по ролям (участники 20, модераторы 21, организаторы 10, жюри 10)
SELECT r.Title AS [Роль], COUNT(*) AS [Кол-во] FROM [User] u JOIN [Role] r ON r.Id = u.RoleId GROUP BY r.Title;
-- 3. Внешние ключи (должно быть 11 строк)
SELECT name AS [FK], OBJECT_NAME(parent_object_id) AS [Таблица], OBJECT_NAME(referenced_object_id) AS [Ссылается на] FROM sys.foreign_keys ORDER BY 2;
-- 4. Ошибки в данных (должно быть пусто)
SELECT 'Модератор не с ролью модератор' [Ошибка], a.Id FROM Activity a JOIN [User] u ON u.Id=a.ModeratorId WHERE u.RoleId<>2
UNION ALL SELECT 'Жюри не с ролью жюри', m.Id FROM Mark m JOIN [User] u ON u.Id=m.JuryId WHERE u.RoleId<>4
UNION ALL SELECT 'Победитель не участник', e.Id FROM [Event] e JOIN [User] u ON u.Id=e.WinnerId WHERE u.RoleId<>1
UNION ALL SELECT 'Телефон не из цифр', Id FROM [User] WHERE Phone LIKE '%[^0-9]%'
UNION ALL SELECT 'Активность > 5 жюри', ActivityId FROM Mark GROUP BY ActivityId HAVING COUNT(*)>5;
-- 5. Мероприятия с направлением, городом и победителем
SELECT e.Id, e.Title, e.[Date], e.Days, c.Title AS City, co.CourseTitle, u.Surname + ' ' + u.Name AS Winner, e.Logo
FROM [Event] e JOIN City c ON c.Id=e.CityId JOIN Course co ON co.Id=e.CourseId LEFT JOIN [User] u ON u.Id=e.WinnerId ORDER BY e.[Date];
GO

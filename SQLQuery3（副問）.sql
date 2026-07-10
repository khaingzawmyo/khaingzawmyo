-- (ñ‚ëË43)
select p.name as playersname ,p.club,p.position,p.height
from players p
inner join(
  select position ,Max(height)  as heighest  from players group by position )as p2
  on p2.position = p.position and p.height = p2.heighest;

-- (ñ‚ëË44)
select p.position,
 (select p2.name from players  p2 
 where p2.position = p.position and 
 p2.height = (select max(height) from players p3  where p3.position = p.position
  ))as playersname,
  (select max(height)  from players as p4  where p4.position = p.position) as heighest
from players p
group by p.position;

Select 
    position AS POSITION,
    (select name
     FROM players_tmp AS sub 
     WHERE sub.position = players_tmp.position AND sub.height = 
           (SELECT MAX(height) FROM players_tmp WHERE position = players_tmp.position)) AS playersname,
    (SELECT MAX(height) 
     FROM players_tmp AS sub 
     WHERE sub.position = players_tmp.position) AS ç≈Ç‡çÇÇ¢êgí∑
FROM 
    players_tmp
GROUP BY 
    position;

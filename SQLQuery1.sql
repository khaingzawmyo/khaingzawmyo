select name,position,height from players
where height < '170' and
(position = 'MF' or position = 'FW');



select id ,concat(name,'選手のポジションは',position,'です。') as player_jouhou from players;			
select * from players;	
select id,name,birth from players
order by birth desc ;

select id,name,height,weight from players
order by height desc ,weight desc;



select *,format(birth,'yyyy年MM月dd日') as birthday from players;

select id ,pairing_id,goal_time,
case
 when player_id Is Null then '9999'
 else
  player_id 
end as player_id2
from goals;

select  AVG(height) as '平均身長', AVG(weight)  as '平均体調'
from players;

select count(*)from goals
where player_id between 714 and 736;

Select				
count(*) as goals_time				
from goals				
where player_id between 714 and 736;			


select name ,(select max(height) from players) as heihgest,
(select max(weight) from players) as weightest 
from players;

select
Max(weight) As 最大体重,
Max(height) As 最大身長
From players;


select * from countries;

select count(player_id) As player_id
from goals;

select name ,( select min(ranking)  from countries  where group_name = 'A' )  from countries
where group_name = 'A';

select sum(ranking) As C_group	
from countries			
where group_name = 'C';		

SELECT 
    name, 
    -- カッコの中にも「FROM」と「WHERE」をしっかり書くのがルールです
    (SELECT Min(ranking) FROM countries WHERE group_name = 'A') AS A_group
FROM countries
WHERE group_name = 'A';

select name,ranking,
(select sum(ranking) from countries where group_name = 'C')
from countries
where group_name = 'C'

SELECT name, ranking,
  -- Cグループのランキング合計値を計算して横に並べる
  (SELECT SUM(ranking) FROM countries WHERE group_name = 'C') AS c_group_total
FROM countries
WHERE group_name = 'C';
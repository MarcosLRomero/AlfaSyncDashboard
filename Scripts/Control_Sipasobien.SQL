USE DISTRIWALTERP 
-- malvinas
select * from (
select a.IDARTICULO, a.DESCRIPCION, a.COSTO, b.costo as costo1 from
v_ma_articulos a inner join
[serverdw].[ALFANET].[dbo].v_ma_articulos b on
a.IDARTICULO = b.IDARTICULO 
 
 ) a
 where COSTO < > costo1
 -- jose c paz
 select * from (
select a.IDARTICULO, a.DESCRIPCION, a.COSTO, b.costo as costo1 from
v_ma_articulos a inner join
[serverdwjcp\ALFANET].[DWJCPAZ].[dbo].v_ma_articulos b on
a.IDARTICULO = b.IDARTICULO 
 
 ) a
 where COSTO < > costo1
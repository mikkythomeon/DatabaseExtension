--<Query Name="GetGITVariance">
--show all the items received at the warehouse that have not been recorded out of production
	with a as (
		Select ProductLocation.Product,Serial,Product.PRODUCTNAME Description,Product.MODEL Model, Product.PRODUCTTYPE ProductType,
		DAY(EntryDate) as TrnDay,MONTH(EntryDate) as TrnMonth,YEAR(EntryDate) as TrnYear ,  ProductLocation.EntryDate 
		from [ProductLocation] LEFT JOIN Product 
		on ProductLocation.Product=Product.PRODUCT 
		where PLine=99 and Location=104 and EntryDate between '@StartDate' and dateadd(d,1,'@EndDate')
	), b as (
		Select ProductLocation.Product,Serial,Product.PRODUCTNAME Description,Product.MODEL Model, Product.PRODUCTTYPE ProductType,
		DAY(EntryDate) as TrnDay,MONTH(EntryDate) as TrnMonth,YEAR(EntryDate) as TrnYear ,  ProductLocation.EntryDate 
		from [ProductLocation] LEFT JOIN Product 
		on ProductLocation.Product=Product.PRODUCT 
		where PLine=99 and Location=105 and EntryDate between '@StartDate' and dateadd(d,1,'@EndDate')
	), c as  (
		select a.*,'Not Scanned into warehouse' Status from a LEFT JOIN b
		on a.Product=b.Product and a.Serial=b.Serial
		where b.Product IS NULL
	), d as (
		select b.*,'Not Scanned out of production' Status from b LEFT JOIN a
		on b.Product=a.Product and b.Serial=a.Serial
		where a.Product IS NULL
	)
	select * from c
	UNION ALL
	select * from d
	order by EntryDate DESC
--</Query>

--<Query Name="GetProductionReceipts">
	Select ProductLocation.Product,Serial,Product.PRODUCTNAME Description,Product.MODEL Model, Product.PRODUCTTYPE ProductType,
	DAY(EntryDate) as TrnDay,MONTH(EntryDate) as TrnMonth,YEAR(EntryDate) as TrnYear ,  ProductLocation.EntryDate ,''STatus
	from [ProductLocation] LEFT JOIN Product 
	on ProductLocation.Product=Product.PRODUCT 
	where PLine=99 and Location=104 and EntryDate between '@StartDate' and dateadd(d,1,'@EndDate')
	order by EntryDate DESC
--</Query>

--<Query Name="SaveProductLocation">
	insert into ProductLocation(PLine,Product,Serial,Location,EntryDate) 
	values('@pline','@product','@serial',@pl,getdate())
--</Query>

--<Query Name="GetWarehouseReceipts">
	Select ProductLocation.Product,Serial,Product.PRODUCTNAME Description,Product.MODEL Model, Product.PRODUCTTYPE ProductType,
	DAY(EntryDate) as TrnDay,MONTH(EntryDate) as TrnMonth,YEAR(EntryDate) as TrnYear ,  ProductLocation.EntryDate ,''STatus
	from [ProductLocation] LEFT JOIN Product 
	on ProductLocation.Product=Product.PRODUCT 
	where PLine=99 and Location=105 and EntryDate between '@StartDate' and dateadd(d,1,'@EndDate')
	order by EntryDate DESC
--</Query>



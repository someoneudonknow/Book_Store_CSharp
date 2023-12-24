use BookStoreManager;
-- trigger when create customer order with initial status
go
CREATE or alter TRIGGER TR_CREATE_CUSTOMER_ORDER_STATUS_FROM_ORDER ON CUSTOMER_ORDER FOR INSERT, update AS
BEGIN
	declare @customerID int;
	declare @OrderPaymentMethod NVARCHAR(50);
	declare @TotalPrice DECIMAL(12,2);
	declare @OrderID int;

	select	@OrderID = i.OrderID,
			@customerID = i.CustomerID, 
			@OrderPaymentMethod = i.OrderPaymentMethod, 
			@TotalPrice = i.OrderTotalPrice from inserted i;
	
	if (select count(*) from deleted) = 0
	begin
		insert into CUSTOMER_ORDER_STATUS VALUES(@orderID, 1, GETDATE());
	end

END

-- trigger to update stock when insert CUSTOMER_ORDER_STATUS
GO
CREATE or alter TRIGGER TR_UPDATE_STOCK_WITH_INSERT ON CUSTOMER_ORDER_STATUS FOR INSERT AS
BEGIN
	DECLARE @orderID int = 0;
	DECLARE @statusID int = 0;
	select @orderID = i.OrderID, @statusID = i.StatusID from inserted i;

	DECLARE @editionID int = 0;
	DECLARE @currentAvailableStock int = 0;
	DECLARE @orderQuantity int = 0;
	DECLARE @MyCursor CURSOR;

	if @statusID = 4 or @statusID = 8--proccessing or cancel by failed delivering
	begin
		BEGIN
			SET @MyCursor = CURSOR FOR
			select c.EditionID, c.DetailQuantity, s.InventoryAvailableStock 
			from CUSTOMER_ORDER_DETAIL c, STOCK_INVENTORY s 
			where c.EditionID = s.EditionID and c.OrderID = @orderID

			OPEN @MyCursor 
			FETCH NEXT FROM @MyCursor 
			INTO @editionID, @orderQuantity, @currentAvailableStock

			WHILE @@FETCH_STATUS = 0
			BEGIN
			  	if(@orderQuantity > @currentAvailableStock)
				begin
					print('The number of quantity is not enough in storage for book id : '+ cast(@editionID as varchar(20)))
					delete CUSTOMER_ORDER_DETAIL where OrderID = @orderID and EditionID = @editionID
				end
				else
				begin
					if @statusID = 4 -- proccessing
					begin
						update STOCK_INVENTORY set InventoryStockOutTotal = InventoryStockOutTotal + @orderQuantity 
						where EditionID = @editionID
					end
					else if @statusID = 8 -- cancel by failed delivering
					begin
						update STOCK_INVENTORY set InventoryStockOutTotal = InventoryStockOutTotal - @orderQuantity 
						where EditionID = @editionID
					end
				end
				FETCH NEXT FROM @MyCursor INTO @editionID, @orderQuantity, @currentAvailableStock
			END; 

			CLOSE @MyCursor ;
			DEALLOCATE @MyCursor;
		END;
	end
END

--trigger to update InventoryAvailableStock when stock updated 
GO
CREATE or alter TRIGGER TR_UPDATE_INVENTORY_AVAILABLE_STOCK ON STOCK_INVENTORY FOR UPDATE AS
BEGIN
	IF UPDATE(InventoryStockInTotal) OR UPDATE(InventoryStockOutTotal)
    BEGIN
		update STOCK_INVENTORY set InventoryAvailableStock = i.InventoryStockInTotal - i.InventoryStockOutTotal
		from inserted i where STOCK_INVENTORY.EditionID = i.EditionID
	end
END

-- trigger for wallet when order is waiting
GO	
create or alter trigger TR_HANDLE_CUSTOMER_ORDER ON CUSTOMER_ORDER_STATUS for INSERT AS
BEGIN
	Declare @orderID int;
	declare @walletID int;
	declare @bankID int;
	declare @statusID int;
	Declare @customerID int;
	select @orderID = i.OrderID, @statusID = i.StatusID from inserted i;
	select @customerID = CustomerID from CUSTOMER_ORDER where OrderID = @orderID;
	
	DECLARE @orderCurrentTotalPrice decimal(12,2) = 0;
	Declare @orderPaymentMethod NVARCHAR(50);
	SELECT @orderCurrentTotalPrice = OrderTotalPrice, @orderPaymentMethod = OrderPaymentMethod FROM CUSTOMER_ORDER where OrderID = @orderID

	if @orderPaymentMethod = 'CURRENT BALANCE'
	begin
	-- Tru tien khi order o trang thai waiting
		IF @statusID = 2
		BEGIN
			-- check current balance
			IF(SELECT COUNT(*) FROM WALLET WHERE CustomerID = @customerID AND Balance >= @orderCurrentTotalPrice ) > 0
			begin
			UPDATE WALLET SET Balance = Balance - @orderCurrentTotalPrice WHERE CustomerID = @customerID
			select @walletID = WalletID from WALLET WHERE CustomerID = @customerID
			select @bankID = BankAccountID from BANK_ACCOUNT WHERE CustomerID = @customerID
			insert into TRANSACTION_DETAILS values (GETDATE(), 'Place order', null, @walletID, null, @orderID)
			print 'OK'
			end
				
			ELSE
			begin
				print('Current balance of CustomerID:  ' + CAST(@customerID as varchar(10)) + ' for OrderID: ' + cast(@orderID as varchar(10)) +' is not enough')
				rollback tran
			end
		END
		-- Cong tien khi order o trang thai cancel by customer hoac cancel by failed delivering
		else IF @statusID = 3 or @statusID = 8
		begin
			UPDATE WALLET SET Balance = Balance + @orderCurrentTotalPrice WHERE CustomerID = @customerID
			select @walletID = WalletID from WALLET WHERE CustomerID = @customerID
			select @bankID = BankAccountID from BANK_ACCOUNT WHERE CustomerID = @customerID
			insert into TRANSACTION_DETAILS values (GETDATE(), 'Refund', null, @walletID, null, @orderID)
			print'not ok'
		end
	end

END



-- trigger to calculate OrderTotalPrice from  DetailCurrentPrice and  DetailQuantity

go
create or alter trigger TR_CALCULATE_TOTAL_PRICE_FROM_ORDER_DETAIL on CUSTOMER_ORDER_DETAIL for insert as
begin
	declare @tierID int ;
	declare @editionID int;
	declare @detailID int
	select @tierID = Person.TierID from inserted i, Person , CUSTOMER_ORDER
	where i.OrderID = CUSTOMER_ORDER.OrderID and CUSTOMER_ORDER.CustomerID = Person.PersonID

	select @editionID = i.EditionID, @detailID = i.OrderDetailID from inserted i;

	declare @currentPromo decimal(4,2) = (select top 1 p.PromotionDiscount from PROMOTION p, BOOK_PROMOTION bp
	where p.PromotionID = bp.PromotionID and bp.EditionID = @editionID and
	getdate() between p.PromotionStartDate and p.PromotionEndDate 
	
	order by p.PromotionDiscount desc) 

	if @currentPromo = null
	begin
		set @currentPromo = 0;
	end

	declare @tierDiscount decimal(4,2) = 0;
	if(@tierID != null)
	begin
		select @tierDiscount = TierDiscount from TIER where TierID = @tierID
	end	
	update CUSTOMER_ORDER_DETAIL set DetailCurrentPrice = DetailCurrentPrice * (100-@tierDiscount)/100 where OrderDetailID = @detailID
	update CUSTOMER_ORDER set OrderTotalPrice =
	OrderTotalPrice + (i.DetailCurrentPrice * i.DetailQuantity)
	from inserted i where CUSTOMER_ORDER.OrderID = i.OrderID
end



select * from WALLET
--select * from CUSTOMER_ORDER_DETAIL
--select * from CUSTOMER_ORDER_STATUS
--select * from CUSTOMER_ORDER
--select * from STOCK_INVENTORY
--exec SP_CREATE_CUSTOMER_ORDER_STATUS 22,4
select * from V_UserRole
select * from AspNetUsers

--DROP TRIGGER TR_CREATE_CUSTOMER_ORDER_STATUS_FROM_ORDER
--drop TRIGGER TR_UPDATE_STOCK_WITH_INSERT
--drop TRIGGER TR_HANDLE_CUSTOMER_ORDER
--drop TRIGGER TR_CALCULATE_TOTAL_PRICE_FROM_ORDER_DETAIL
--drop TRIGGER TR_UPDATE_INVENTORY_AVAILABLE_STOCK
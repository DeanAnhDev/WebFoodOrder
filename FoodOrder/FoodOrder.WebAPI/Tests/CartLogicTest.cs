// Test case cho logic tính tiền trong Cart
// Để kiểm tra xem logic có hoạt động đúng không

/*
Test scenarios:
1. ✅ Không có khuyến mãi: FinalPrice = OriginalPrice
2. ✅ Khuyến mãi giảm theo số tiền: FinalPrice = OriginalPrice - DiscountAmount
3. ✅ Khuyến mãi giảm theo phần trăm: FinalPrice = OriginalPrice * (1 - DiscountPercent/100)
4. ✅ Khuyến mãi đã hết hạn: FinalPrice = OriginalPrice
5. ✅ Khuyến mãi chưa bắt đầu: FinalPrice = OriginalPrice
6. ✅ Khuyến mãi không active: FinalPrice = OriginalPrice
7. ✅ Giảm giá quá mức (giá âm): FinalPrice = 0

Example test data:

Food 1: Bánh mì - 25,000 VND
- Khuyến mãi: Giảm 5,000 VND
- Expected: 20,000 VND

Food 2: Phở - 50,000 VND  
- Khuyến mãi: Giảm 20%
- Expected: 40,000 VND

Food 3: Cơm - 30,000 VND
- Không khuyến mãi
- Expected: 30,000 VND

Combo 1: Cơm + Canh - 45,000 VND
- Khuyến mãi: Giảm 15%
- Expected: 38,250 VND

Cart Summary:
- Original Total: 150,000 VND
- Final Total: 128,250 VND  
- Total Discount: 21,750 VND
*/
# API GetCartById - Lấy thông tin cart theo ID

## Mô tả

API này được sử dụng để lấy thông tin đầy đủ của một cart bao gồm:

- Thông tin cơ bản của cart (CartId, UserId)
- Danh sách CartItems với đầy đủ thông tin
- Thông tin Food/Combo kèm theo hình ảnh và promotion
- Tính toán giá gốc, giá sau khuyến mãi, và tổng tiền

## Endpoint

```
GET /api/cart/{cartId}
```

## Tham số

- `cartId` (int): ID của cart cần lấy thông tin

## Response Format

### Thành công (200 OK)

```json
{
  "success": true,
  "message": "Lấy thông tin cart thành công",
  "data": {
    "cart": {
      "cartId": 1,
      "userId": 123,
      "cartItems": [
        {
          "cartItemId": 1,
          "cartId": 1,
          "foodId": 10,
          "food": {
            "foodId": 10,
            "foodCategoryId": 1,
            "foodName": "Phở Bò",
            "slug": "pho-bo",
            "description": "Phở bò truyền thống",
            "price": 50000,
            "status": true,
            "quantity": 100,
            "sold": 25,
            "createdAt": "2024-09-23T10:00:00Z",
            "images": {
              "imageId": 1,
              "imagePath": "/images/pho-bo.jpg",
              "altText": "Phở Bò"
            },
            "foodCategory": {
              "foodCategoryId": 1,
              "categoryName": "Món chính"
            },
            "promotion": {
              "promotionId": 1,
              "promotionName": "Giảm giá 10%",
              "discountAmount": 10,
              "type": "Percentage",
              "startDate": "2024-09-01T00:00:00Z",
              "endDate": "2024-09-30T23:59:59Z",
              "isActive": true
            }
          },
          "comboId": null,
          "combo": null,
          "quantity": 2,
          "originalPrice": 50000,
          "finalPrice": 45000,
          "originalTotal": 100000,
          "finalTotal": 90000,
          "discountAmount": 10000
        }
      ]
    },
    "totalQuantity": 2,
    "subtotal": 90000,
    "originalTotal": 100000,
    "totalDiscount": 10000
  }
}
```

### Không tìm thấy (404 Not Found)

```json
{
  "message": "Không tìm thấy cart với ID này"
}
```

### Lỗi server (500 Internal Server Error)

```json
{
  "success": false,
  "message": "Có lỗi xảy ra khi lấy thông tin cart",
  "detail": "Chi tiết lỗi..."
}
```

## Đặc điểm

- API này không yêu cầu authentication
- Trả về đầy đủ thông tin food/combo kèm hình ảnh
- Tự động tính toán giá sau khuyến mãi
- Kiểm tra thời gian hiệu lực của promotion
- Hỗ trợ cả food và combo trong cùng một cart

## Ví dụ sử dụng

### JavaScript/jQuery

```javascript
$.ajax({
  url: "/api/cart/1",
  method: "GET",
  success: function (response) {
    if (response.success) {
      console.log("Cart info:", response.data);
      console.log("Total items:", response.data.totalQuantity);
      console.log("Subtotal:", response.data.subtotal);
    }
  },
});
```

### cURL

```bash
curl -X GET "https://localhost:7001/api/cart/1" \
     -H "Content-Type: application/json"
```

### C# HttpClient

```csharp
using (var client = new HttpClient())
{
    var response = await client.GetAsync("/api/cart/1");
    if (response.IsSuccessStatusCode)
    {
        var content = await response.Content.ReadAsStringAsync();
        // Parse JSON content
    }
}
```

## Cấu trúc dữ liệu trả về

### CartResponse

- `Cart`: Thông tin chi tiết của cart
- `TotalQuantity`: Tổng số lượng items trong cart
- `Subtotal`: Tổng tiền sau khuyến mãi
- `OriginalTotal`: Tổng tiền gốc (trước khuyến mãi)
- `TotalDiscount`: Tổng số tiền được giảm

### CartItemDto

- `OriginalPrice`: Giá gốc của từng item
- `FinalPrice`: Giá sau khuyến mãi của từng item
- `OriginalTotal`: Tổng tiền gốc cho item (OriginalPrice × Quantity)
- `FinalTotal`: Tổng tiền sau khuyến mãi cho item (FinalPrice × Quantity)
- `DiscountAmount`: Số tiền được giảm cho item

## Lưu ý

- API sẽ trả về null nếu cart không tồn tại
- Promotion chỉ được áp dụng nếu đang trong thời gian hiệu lực
- Hỗ trợ cả percentage và amount discount
- Dữ liệu bao gồm đầy đủ thông tin để hiển thị UI cart

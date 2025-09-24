# Get All Foods and Combos API

## Mô tả

API này cho phép lấy tất cả danh sách Food và Combo đang hoạt động (Status = true) kèm theo đầy đủ thông tin Images và Promotion. API được thiết kế để cung cấp dữ liệu hoàn chỉnh cho frontend display.

## Endpoint

**GET** `/api/food/all-foods-and-combos`

## Authorization

- **Không yêu cầu authentication** - API public cho hiển thị sản phẩm

## Request

- **Method**: GET
- **Headers**:
  - `Content-Type: application/json`
- **Body**: Không cần body

## Response

### Success Response (200 OK)

```json
{
  "success": true,
  "message": "Lấy danh sách food và combo thành công",
  "data": {
    "foods": [
      {
        "foodId": 1,
        "foodCategoryId": 1,
        "foodName": "Bánh mì thịt nướng",
        "slug": "banh-mi-thit-nuong",
        "description": "Bánh mì với thịt nướng thơm ngon",
        "price": 25000,
        "status": true,
        "quantity": 100,
        "sold": 50,
        "createdAt": "2024-01-01T10:00:00Z",
        "images": {
          "imageId": 1,
          "imageName": "banh-mi-thit-nuong.jpg",
          "imageUrl": "https://example.com/images/banh-mi-thit-nuong.jpg",
          "alt": "Bánh mì thịt nướng"
        },
        "foodCategory": {
          "categoryId": 1,
          "categoryName": "Bánh mì",
          "description": "Các loại bánh mì"
        },
        "promotion": {
          "promotionId": 1,
          "promotionName": "Giảm giá 10%",
          "discountPercent": 10,
          "startDate": "2024-01-01T00:00:00Z",
          "endDate": "2024-12-31T23:59:59Z",
          "status": true
        }
      }
    ],
    "combos": [
      {
        "comboId": 1,
        "comboName": "Combo bánh mì + nước",
        "slug": "combo-banh-mi-nuoc",
        "foodCategoryId": 2,
        "price": 35000,
        "description": "Combo tiết kiệm bánh mì kèm nước uống",
        "status": true,
        "quantity": 50,
        "sold": 25,
        "createdAt": "2024-01-01T10:00:00Z",
        "images": {
          "imageId": 2,
          "imageName": "combo-banh-mi-nuoc.jpg",
          "imageUrl": "https://example.com/images/combo-banh-mi-nuoc.jpg",
          "alt": "Combo bánh mì nước"
        },
        "foodCategorys": {
          "categoryId": 2,
          "categoryName": "Combo",
          "description": "Các combo tiết kiệm"
        },
        "promotion": {
          "promotionId": 2,
          "promotionName": "Giảm giá 15%",
          "discountPercent": 15,
          "startDate": "2024-01-01T00:00:00Z",
          "endDate": "2024-12-31T23:59:59Z",
          "status": true
        }
      }
    ],
    "totalFoods": 1,
    "totalCombos": 1,
    "totalItems": 2
  }
}
```

## Error Responses

### 500 Internal Server Error

```json
{
  "success": false,
  "message": "Có lỗi xảy ra khi lấy danh sách food và combo",
  "detail": "Chi tiết lỗi..."
}
```

## Ví dụ sử dụng

### 1. Lấy tất cả Foods và Combos

```http
GET /api/food/all-foods-and-combos
Content-Type: application/json
```

**Response:**

```json
{
  "success": true,
  "message": "Lấy danh sách food và combo thành công",
  "data": {
    "foods": [...],
    "combos": [...],
    "totalFoods": 15,
    "totalCombos": 8,
    "totalItems": 23
  }
}
```

## Data Structure Details

### FoodDto Structure

- `foodId`: ID của món ăn
- `foodCategoryId`: ID danh mục món ăn
- `foodName`: Tên món ăn
- `slug`: URL-friendly string
- `description`: Mô tả món ăn
- `price`: Giá tiền (decimal)
- `status`: Trạng thái hoạt động (boolean)
- `quantity`: Số lượng tồn kho
- `sold`: Số lượng đã bán
- `createdAt`: Thời gian tạo
- `images`: Thông tin hình ảnh (ImageDto)
- `foodCategory`: Thông tin danh mục (FoodCategoryDto)
- `promotion`: Thông tin khuyến mãi (PromotionDtoSelect)

### ComboDto Structure

- `comboId`: ID của combo
- `comboName`: Tên combo
- `slug`: URL-friendly string
- `foodCategoryId`: ID danh mục
- `price`: Giá combo (decimal)
- `description`: Mô tả combo
- `status`: Trạng thái hoạt động (boolean)
- `quantity`: Số lượng tồn kho
- `sold`: Số lượng đã bán
- `createdAt`: Thời gian tạo
- `images`: Thông tin hình ảnh (ImageDto)
- `foodCategorys`: Thông tin danh mục (FoodCategoryDto)
- `promotion`: Thông tin khuyến mãi (PromotionDtoSelect)

### ImageDto Structure

- `imageId`: ID hình ảnh
- `imageName`: Tên file hình ảnh
- `imageUrl`: URL đầy đủ của hình ảnh
- `alt`: Mô tả alt cho hình ảnh

### PromotionDtoSelect Structure

- `promotionId`: ID khuyến mãi
- `promotionName`: Tên khuyến mãi
- `discountPercent`: Phần trăm giảm giá
- `startDate`: Ngày bắt đầu khuyến mãi
- `endDate`: Ngày kết thúc khuyến mãi
- `status`: Trạng thái khuyến mãi

## Business Logic

### Filtering Rules:

1. **Active Items Only**: Chỉ lấy Foods và Combos có `status = true`
2. **Complete Data**: Bao gồm đầy đủ Images và Promotion information
3. **Category Information**: Kèm theo thông tin danh mục sản phẩm
4. **Stock Information**: Hiển thị số lượng tồn kho và đã bán

### Performance Considerations:

- Sử dụng Entity Framework Include để eager loading
- Tối ưu query với single database call cho mỗi entity type
- AutoMapper để mapping hiệu quả từ Entity sang DTO

## Use Cases

### Frontend Applications:

- **Product Listing Page**: Hiển thị tất cả sản phẩm có sẵn
- **Search & Filter**: Cung cấp data cho tính năng tìm kiếm
- **Category Navigation**: Hiển thị sản phẩm theo danh mục
- **Promotion Display**: Hiển thị giá gốc và giá sau khuyến mãi

### Mobile Applications:

- **Home Screen**: Hiển thị danh sách sản phẩm nổi bật
- **Menu Display**: Hiển thị menu đầy đủ với hình ảnh
- **Offline Support**: Cache data cho offline browsing

### E-commerce Integration:

- **Shopping Cart**: Hiển thị thông tin sản phẩm khi thêm vào giỏ
- **Product Comparison**: So sánh giữa Foods và Combos
- **Recommendation System**: Gợi ý sản phẩm dựa trên dữ liệu

## Related APIs

### Comparison với các APIs khác:

- **vs GET /api/food**: API này chỉ lấy foods với pagination
- **vs GET /api/combo/combos**: API này chỉ lấy combos
- **vs GET /api/food/{id}**: API này lấy chi tiết 1 food
- **vs GET /api/combo/{id}**: API này lấy chi tiết 1 combo

### Lợi ích của API tổng hợp:

- **Giảm số lượng API calls**: 1 call thay vì 2+ calls
- **Dữ liệu đồng bộ**: Đảm bảo consistency giữa foods và combos
- **Performance**: Tối ưu cho mobile và slow connections
- **Simplicity**: Đơn giản hóa logic frontend

## Status Codes

- `200 OK`: Lấy dữ liệu thành công
- `500 Internal Server Error`: Lỗi server

## Dependencies

- **FoodServices**: Service xử lý business logic
- **ComboServices**: Service lấy dữ liệu combo
- **AutoMapper**: Mapping entities sang DTOs
- **Entity Framework**: Database access với eager loading

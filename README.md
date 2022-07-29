"# ElasticsearchIssuesDemo" 

### step1
```
// 首先，插入10万条测试数据
// First, insert 100000 pieces of test data
```

### step2
http://localhost:5000/js
```
// 使用原生ajax方式访问restful接口可以获取到5千条数据，尽管有些数据不存在
// Using native Ajax to access restful interface can get 5000 pieces of data, although some data does not exist
```

### step3
http://localhost:5000/sdk
```
// 使用SDK方式无法获取到5千条数据，出现bug
// Unable to get 5000 pieces of data using SDK, bug1
```
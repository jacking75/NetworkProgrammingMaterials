# Golang - shamaton/msgpack 라이브러리
- [GitHub](https://github.com/shamaton/msgpack) 
  
## MessagePack 시리얼라이저(msgpack)를 만들었다
[출처](https://shamaton.orz.hm/blog/archives/570)  
  
이미 Go 언어로 MessagePack가 가능한 패키지가 이미 일부 존재하고 사용하고 싶다면 사용할 수 있는 상황이다. 나도 사용하고 있는데, 어떤 계기로 package를 들여다 보았는데, 어쩐지 좀 더 빨리 할 수 있을 것 같은데... 라고 생각했다.  
  
간단한 검증 벤치 마크 코드를 작성 보았는데, 빨라졌나...라고 하는 것으로 쓰기 시작한 것이 처음이었다. 코드를 재 작성 벤치 마크를 확인하고... 구현하고... 반복 진행하고 있으며, 현재(2018.09 기준) v1.0로 출시 되었다.  
  
엔드 포인트의 명칭은 고민 했지만, 여러가지 생각 끝에 Encode/Decode로 했다.  
호출은 이런 느낌으로.  
 sample.goGo  
```
package main;

import (
  "github.com/shamaton/msgpack"
)

func main() {
    type Struct struct {
        String string
    }
    v := Struct{String: "msgpack"}

    d, err := msgpack.Encode(v)
    if err != nil {
        panic(err)
    }
    r := Struct{}
    err = msgpack.Decode(d, &r)
    if err != nil {
        panic(err)
    }
}
```  
    
단순하게 사용만 한다면 Encode/Decode를 호출 할뿐이다!  
인수는 표준 패키지 encoding/json과 같은 형태로 하고 있다.(대체가 쉬울지도 ...!)  
  
그리고, 중요한 성능이지만, 우선 직렬화.  
내가 알고 있는 범위에서 다른 MessagePack 및 JSON 등의 비슷한 다른 포맷과 비교 하였다. 벤치 마크는 아마도 사용자가 호출하는 엔드포인트로 해 보았다.  
<pre>
$ go test -bench CompareEncode -benchmem
goos: darwin
goarch: amd64
pkg: github.com/shamaton/msgpack_bench
BenchmarkCompareEncodeShamaton-4             1000000          1255 ns/op         320 B/op          3 allocs/op
BenchmarkCompareEncodeVmihailenco-4           300000          4645 ns/op         968 B/op         14 allocs/op
BenchmarkCompareEncodeArrayShamaton-4        1000000          1110 ns/op         256 B/op          3 allocs/op
BenchmarkCompareEncodeArrayVmihailenco-4      300000          4387 ns/op         968 B/op         14 allocs/op
BenchmarkCompareEncodeUgorji-4               1000000          1921 ns/op         986 B/op         11 allocs/op
BenchmarkCompareEncodeZeroformatter-4        1000000          1890 ns/op         744 B/op         13 allocs/op
BenchmarkCompareEncodeJson-4                  500000          3428 ns/op        1224 B/op         16 allocs/op
BenchmarkCompareEncodeGob-4                   200000         11537 ns/op        2824 B/op         50 allocs/op
BenchmarkCompareEncodeProtocolBuffer-4        500000          2338 ns/op         792 B/op         29 allocs/op
PASS
ok      github.com/shamaton/msgpack_bench   14.481s
</pre>  
    
Shamaton 리고 붙어 있는 것이 이번에 출시 한 것이다. ArrayShamaton이란 MessagePack 형식이 가벼운 패턴을 채용하고 있는 것이다(Unity에서는 MessagePack-CSharp 가 친숙할 듯?). 이번 경우에는 일반 Encode도 다른 패키지보다 성능 잘 나왔다.  
  

다음 직렬화이다.  
<pre>
$ go test -bench CompareDecode -benchmem
goos: darwin
goarch: amd64
pkg: github.com/shamaton/msgpack_bench
BenchmarkCompareDecodeShamaton-4             1000000          1393 ns/op         512 B/op          6 allocs/op
BenchmarkCompareDecodeVmihailenco-4           300000          5393 ns/op        1056 B/op         33 allocs/op
BenchmarkCompareDecodeArrayShamaton-4        2000000           990 ns/op         512 B/op          6 allocs/op
BenchmarkCompareDecodeArrayVmihailenco-4      300000          4397 ns/op         992 B/op         22 allocs/op
BenchmarkCompareDecodeUgorji-4                500000          2587 ns/op         845 B/op         12 allocs/op
BenchmarkCompareDecodeZeroformatter-4        1000000          2350 ns/op         976 B/op         29 allocs/op
BenchmarkCompareDecodeJson-4                  200000          8904 ns/op        1216 B/op         43 allocs/op
BenchmarkCompareDecodeGob-4                    50000         34805 ns/op       10172 B/op        275 allocs/op
BenchmarkCompareDecodeProtocolBuffer-4       1000000          1759 ns/op         656 B/op         19 allocs/op
PASS
ok      github.com/shamaton/msgpack_bench   16.946s
</pre>  
    
이쪽도 성능 좋게 동작 하였다. 단지 Protocol Buffer는 데이터의 형태에 따라 성능 좋아질 수 있었다(struct only 구성 등). 아무튼 proto 파일 이라든지를 준비하지 않아도 되기 때문에 이 점은 좋았다.  
  
결과를 보면 알지만 Array 호출이 성능이 역시 좋기 때문에 사용할 수 있다면 AsArray를 사용하는 것이 좋을 것 일라고 생각한다. 벤치 마크이지만, 여기에 코드가 있기 때문에 무엇인가 있으면 가르쳐 주기 바란다.  
https://github.com/shamaton/msgpack_bench  
   
이것으로, 간단한 호출로 MessagePack 계에서는 비교적 빠른 시리얼을 만들 수 있었다. 좋았다 좋았다. 그것과 이번의 구현으러 [zeroformatter](https://shamaton.orz.hm/blog/archives/570) 측에도 반영 할 것 같은 곳도 알고 있으므로 대응할 수 있으면 -라고 생각하고 있다.  
  
그리고 여담이지만, 태그 직렬화 대상을 설정할 수 있다든가, 확장 인코딩 디코딩도 지원 시키기도 하고 있다.(이 부분은 README 라든지 Example에 쓸 예정이다)  
    
    
<br>  


## Go로 MessagePack 코드를 생성하는 패키지(msgpackgen)를 만들었다
[출처](https://zenn.dev/shamaton/articles/18e6f979627e76  )
  
### 왜 만들었나?  
[entity에서 코드 자동 생성 한 이야기 ​​- Money Forward Kessai TECH BLOG](https://tech.mfkessai.co.jp/2019/09/ebgen/)  
아, 이것을 쓰면 전에 생각해 본 적을 만들 수 있을지도...라고 생각하고, 만들기 시작해서 완성된 것이 msgpackgen 이다.  
   
### 벤치 마크
ShamatonGen과 suffix에 붙어있는 것이 이번에 소개하는 것이다. 아는 범위에서 Go 언어용 MessagePack 시리얼 라이저와 공식 Protobuf 와 Json 을 넣어서 비교해 보았다. 지금의 가장 빠른 것이다...!

결과가 비슷한 tinylib라는 패키지도 Code Generator가 있는 시리얼라이저이다. 구현시 비교 대상으로 하고 있었지만, 그쪽보다 쉽게 코드 생성 될 수 있는 구조로 하는 것을 노력했다.  
  
### 사용법 - 코드 생성
우선 코드를 생성 해 보자. Go는 go generate라는 코드 생성을 하는 구조가 있고, 파일에 정해진 의견을 기재해 두는 것으로, 효력을 발휘하여준다. 예를 들어 main.go에  
```
//go:generate go run github.com/shamaton/msgpackgen
```    
라고 기재하고, 쉘에서     
  
<pre>
go generate
</pre>  
로 하면 main.go이 있는 디렉토리를 포함하여 재귀적으로 자식 디렉토리까지 코드 생성 가능한 구조체(struct)를 검출하고 코드를 생성한다. 출력 하는 파일은 1 파일에 정리하고, 기본 파일 이름은 `resolver.msgpackgen.go` 으로 생성된다.  
  
생성 예: [이것](https://github.com/shamaton/msgpack_bench/blob/master/struct.go)은 [이렇게](https://github.com/shamaton/msgpack_bench/blob/master/resolver.msgpackgen.go)  되었다.  
  

### 사용법 - 직렬화
직렬화를 사용하는 응용 프로그램의 시작 단계 등에서 사용하는 메소드를 불러 놓고 있다. 예를 들어 main.go 등으로 ...     
```
func main() {
	// 登録 (resolver.msgpackgen.goにある)
	RegisterGeneratedResolver()
	
	// http.Serveするなど
	service()
}
```  
  
직렬화/역 직렬화는 json 패키지처럼 Marshal/Unmarshal이 준비 되어 있지다. import에 github.com/shamaton/msgpackgen/msgpack를 추가한다.  
```
func service() {
    v := RegisteredStruct{}
    b, err := msgpack.Marshal(v)
    if err != nil {
        panic(err)
    }
    
    var vv RegisteredStruct
    err = msgpack.Unmarshal(b, &vv)
    if err != nil {
        panic(err)
    }
}
```
  
만약 resolver.msgpackgen.go에 등록 되지 않은 데이터가 인수로 전달된 경우에는 본 [msgpack](https://github.com/shamaton/msgpack) 호출 시리얼라이즈 된다.  
  


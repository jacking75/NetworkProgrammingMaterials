# MsgPack 
- [공식 사이트(일어)](https://msgpack.org  )  
- [공식 사이트(일어)](https://msgpack.org/ja.html  )    

<br>  
<br>  

CWPack https://github.com/clwi/CWPack  
Protocol Buffer と Message Pack の違いとベンチマークを比較 https://uqichi.net/posts/protocol-buffer-message-pack/  
Introducing the MessagePack http://frsyuki.hatenablog.com/entry/20100324/p1  
バイナリシリアライズ形式「MessagePack」 http://frsyuki.hatenablog.com/entry/20080816/p1  
gumiStudy#7 The MessagePack Project(29~34) https://www.slideshare.net/frsyuki/gumistudy7-the-messagepack-project  
MessagePackフォーマット仕様にTimestamp型を追加 http://frsyuki.hatenablog.com/entry/2017/08/10/144310  
사양 소개 https://qiita.com/mikiya0417/items/d86ca4c28b5946015a85  
MessagePackバイナリを自分でデコードして読む https://qiita.com/gecko655/items/ca4f621a3ec5c5f35394  
Redis x MessagePack https://developers.microad.co.jp/entry/2018/12/20/165034  
  
  
# 활용 
    
## C++ 예제 코드  
```
#include <msgpack.hpp>
#include <vector>
#include <string>
#include <fstream>
#include <iostream>

using namespace std;

struct User {
  User(std::string name, int id, std::vector<int> follower)
    : name(name)
    , id(id)
    , follower_id(follower)
  {}

  // default 생성자 필수
  User()
    : id(0)
  {}

  MSGPACK_DEFINE(name, id, follower_id);
  std::string name;
  int id;
  std::vector<int> follower_id;

  string toString(){
    string ret = "name: ";
    ret += name;
    ret += "  id: ";
    ret += std::to_string(id);
    ret += "  follower: ";
    for(int i=0; i<follower_id.size(); ++i){
      ret += std::to_string(follower_id[i]);
      ret += " ";
    }
    return ret;
  }
};

int main(void) {
  // 데이터를 만들고 쓴다
  std::vector<int> follower_1;
  follower_1.push_back(1);
  follower_1.push_back(2);

  std::vector<int> follower_2;
  follower_2.push_back(2);

  std::vector<User> users;
  users.push_back(User("user1", 1, follower_1));
  users.push_back(User("user2", 2, follower_2));

  msgpack::sbuffer sbuf;
  msgpack::pack(sbuf, users);

  ofstream myFile ("cpp_data.bin", ios::out | ios::binary);
  myFile.write(sbuf.data(), sbuf.size());
  myFile.close();


  // 파일에서 데이터를 읽는다
  char buffer[1000];
  ifstream inFile ("ruby_data.bin", ios::in | ios::binary);
  inFile.read (buffer, 1000);
  inFile.close();

  msgpack::unpacked msg;
  msgpack::unpack(&msg, buffer, 1000);

  msgpack::object obj = msg.get();
  std::vector<User> load_users;
  obj.convert(&load_users);
  for(int i=0; i<load_users.size(); ++i){
    cout << load_users[i].toString() << endl;
  }

  return 0;
}
```    
  
- (일어)C++에서 MessagePack 사용하기 [1](https://zv-louis.hatenablog.com/entry/2018/10/20/103000), [2](https://zv-louis.hatenablog.com/entry/2018/10/23/060000), [3](https://zv-louis.hatenablog.com/entry/2018/10/25/190000), [4](https://zv-louis.hatenablog.com/entry/2018/11/04/130000), [5](https://zv-louis.hatenablog.com/entry/2018/11/05/200000), [6](https://zv-louis.hatenablog.com/entry/2018/11/15/000427)


## Golang
  
### tinylib/msgp  
- https://github.com/tinylib/msgp
- 구조체를 선언하면 msgpack 시리얼라이즈/디시리얼라이즈 코드를 생성한다
- mgsp를 라이브러리로 사용하여 msgpack 시리얼라이즈/디시리얼라이즈 할 수 있다. 
    - https://github.com/tinylib/msgp/wiki/Getting-Started    
     
### [shamaton/msgpack](./go-shamaton_msgpack.md)

        
### [(일어) Go로 쓴 MessagePack 패키지 소개](https://zenn.dev/nnabeyang/articles/27d6af9089ea0e )



## C#
- https://github.com/msgpack/msgpack-cli  


# 라이브러리 만들기
- MessagePack-CSharp https://github.com/neuecc/MessagePack-CSharp  유니티에서 사용할 때의 툴로 코드 제너레이터한 후 이것을 기반으로 원본 코드를 분석하는 것이 좋다  
- C#의 MessagePack과 호환되는 C++, Golang 라이브러리 만들기. MessagePack의 msc.exe로 만들어지는 코드를 참고하여 C++, Golang도 이런 코드가 만들어지도록 하기


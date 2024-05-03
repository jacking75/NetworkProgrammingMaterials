# eBPF 분석 및 정리
- [eBPF Blog](https://ebpf.io/  )
  
# 설치 & 기본 사용
- [(일어) ubuntu 20.04.3 LTS에서 eBPF 설치하기](https://qiita.com/hayama17/items/d06d11c47f1803cc9e1d  )
- [(일어) Amazon Linux 2 환경에서 eBPF 프로그래밍으로 Hello World 해보기](https://dev.classmethod.jp/articles/hello-world-with-ebpf-on-amazon-linux-2 )
  

# 학습    
- [eBPF와 함께 이해하는 Cilium 네트워킹](https://speakerdeck.com/hadaney/ebpfwa-hamgge-ihaehaneun-cilium-neteuweoking )   
- [(일어) eBPF은 무엇이 기쁜가?](https://speakerdeck.com/yutarohayakawa/ebpfhahe-gaxi-siinoka )   
- [BCC Contents]([)https://github.com/iovisor/bcc#contents )
- [BPF 스터디 노트](https://www.bhral.com/post/linux-kernel-bpf-%EC%8A%A4%ED%84%B0%EB%94%94-%EB%85%B8%ED%8A%B8 ) 
- [(일어) eBPF 입문](https://zenn.dev/masibw/articles/068bdfe5edee17 )
- [(일어) eBPF - 입문 개요](https://zenn.dev/hidenori3/articles/e1352e8cfeb2af  )
    - [(일어) 가상 머신](https://zenn.dev/hidenori3/articles/cb8ddfb964bbc5 )    
    - [(일어) BCC 튜토리얼](https://zenn.dev/hidenori3/articles/186861b2d5220b )
    - [(일어) bpftrace 튜토리얼](https://zenn.dev/hidenori3/articles/b1cdf7613822d7 )
    - [(일어) XDP 개요](https://zenn.dev/hidenori3/articles/21c4540b80597d )
- [(일어) eBPF 시작하는 방법/Learn eBPF](https://speakerdeck.com/chikuwait/learn-ebpf )
- [(일어) eBPF에 3일간 입무한 이야기](https://caddi.tech/archives/3880   )
    - 역사와 어떤 용도로 사용하면 좋을지 설명 되어 있음  
- [(일어) Berkeley Packet Filter(BPF) 입문](https://atmarkit.itmedia.co.jp/ait/series/11804/  )  


# bpftrace 
- bpftrace ワンライナーチュートリアル  https://github.com/iovisor/bpftrace/blob/master/docs/tutorial_one_liners_japanese.md
- [(회사 위키) eBPF + BCC를 이용한 성능 분석](https://jira.com2us.com/wiki/pages/viewpage.action?pageId=140545983 )


# [bcc(BPF Compiler Collection)](https://github.com/iovisor/bcc)
- eBPF를 위한 Python/Lua 프레임워크
  
    
# Golang
[출처](https://blog.yuuk.io/entry/2021/ebpf-tracing )     
Prometheus로 대표되는 것처럼 Go로 작성된 Observability 도구는 다수 존재한다. Go로 BPF의 프런트엔드를 쓰고 싶다는 요구도 있을 것이다.  
  
Go에서 BPF 프런트엔드를 작성하려면 아래 라이브러리 중 하나를 사용한다. 프런트엔드의 BPF 라이브러리에 최소한의 필요한 처리는 (1)BPF 바이트 코드와 map의 커널로의 로딩과  (2)map의 조작이다.  
- [iovisor/gobpf](https://github.com/iovisor/gobpf ): BCC의 Go 래퍼
- [dropbox/goebpf](https://github.com/dropbox/goebpf ): libbpf를 사용하지 않고 직접 bpf 시스템 호출
- [cilium/ebpf](https://github.com/cilium/ebpf ): Pure Go
- [DataDog/ebpf](https://github.com/DataDog/ebpf ): cilium/ebpf에서 fork 되어, BPF 객체의 라이프사이클 관리 매니저가 추가되고 있다.
- [aquasecurity/libbpfgo](https://github.com/aquasecurity/libbpfgo ): 원래는 보안 런타임 [Tracee](https://aquasecurity.github.io/tracee/latest )용의 libbpf Go 래퍼.  
    - [libbpfgo-beginners](https://github.com/lizrice/libbpfgo-beginners )
    - [(일어) libbpf에 의한 eBPF 프로그램 만드는 방법](https://www.creationline.com/lab/aquasecurity/41901 )
- libbpf + cgo bindings  
  
커널이 제공하는 BPF의 최신 기능을 사용하고 싶다면, 커널의 업스트림으로 유지되고 있는 libbpf를 사용한다.  
Go에서 cgo를 사용하여 libbpf API를 호출한다. libbpf를 Go의 바이너리에 포함하려면 libbpf를 정적 링크한다. 구체적으로는, [libbpf의 정적 라이브러리 파일(.a)을 CGO_LDFLAGS로 지정하여 빌드](https://github.com/yuuki/go-conntracer-bpf/blob/e36514323db7b9b84abdced2ba0710ac5468f8d0/Makefile#L89-L94 )한다.   
libbpf는 libelf와 libz에 의존하기 때문에, 이러한 패키지가 인스톨되어 있지 않은 환경을 상정한다면, libelf와 libz도 스스로 빌드해서 바이너리에 포함한다.  
  
libbpf API를 직접 호출하는 것이 번거롭다면 aquasecurity/libbpfgo를 사용한다. 다만, libbpf의 모든 API가 랩핑 되고 있는 것은 아니기 때문에, 사용하고 싶은 API가 서포트 되고 있는지를 확인해야 한다.  
  
Pure Go 라이브러리를 사용하고 싶다면 cilium/ebpf 또는 DataDog/ebpf를 사용합니다. 다만, 현(2021년) 시점에서는 CO-RE에 대응할 수 없는 등의 과제가 있다.  
  
Go + BPF에 대해서는 다음 기사에도 정리되어 있다. [Getting Started with eBPF and Go | networkop](https://networkop.co.uk/post/2021-03-ebpf-intro/ )  
  
또, XDP에 포커스했을 때의 Go 라이브러리의 선택에 대해서는 @takemioIO 씨에 의한 아래의 기사가 참고가 될 것이다.   
[Go + XDP 개발을 시작할 때 도움이 될 기사](https://takeio.hatenablog.com/entry/2021/01/26/180129 )  
  

# Rust 언어로 BPF 프로그래밍
시스템 소프트웨어용의 프로그래밍 언어로서 Rust가 인기이다. Rust에서 BPF 프로그래밍을 하고 싶은 사람은 많을 것이다. 저자는 Rust의 프로그래밍 경험이 거의 없기 때문에 기존의 리소스를 간단하게 소개하는 것에 머물러 둔다.  
  
[libbpf/libbpf-rs](https://github.com/libbpf/libbpf-rs ) 는 libbpf의 Rust 래퍼이다. libbpf에 의존하지만 libbpf의 최신 기능이 사용하기 쉽습니다.  
  
[aya-rs/aya](https://github.com/aya-rs/aya )는 Rust에서 프런트엔드 프로그램을 작성하기 위한 최근의 BPF 라이브러리이다. aya는 libbpf와 bcc에 의존하지 않고 libc만의 의존으로 CO-RE에 대응하는 바이너리를 생성할 수 있다.  
- 오퍼레이션과 개발자 경험에 초점을 맞추고, BPF Type Format(BTF)을 지원한다.
- bpf-to-bpf 함수 콜과 글로벌 변수 등도 지원하고, eBPF 프로그램을 다른 프로그램처럼 구조할 수 있다. 
- arrays, hash maps 등 다수의 맵, 프로그램을 지원한다. 
- async 지원도 옵트인 형식으로 제공한다. 
- Linux 시스템 콜 API 상에 구축한 표준 C 라이브러리 구현인 musl과 링크하는 것으로, “한 번 컴파일하면, 어디에서나 동작한다”를 실현하고 있다  
      
[foniod/redbpf](https://github.com/foniod/redbpf )는 프런트엔드가 아니라 BPF 프로그램을 Rust로 작성하기 위한 툴과 라이브러리이다.  
  
그 외, Rust에 의한 BPF 트레이싱에 대해서, id:udzura 씨의 다음의 슬라이드가 참고가 된다. [Rust로 만드는 리눅스 트레이서 / libbpf-core-with-rust - Speaker Deck](https://speakerdeck.com/udzura/libbpf-core-with-rust )  
    
    
  
# 응용
- [Linux 게임 서버 성능 평가 (eBPF + BCC)](https://rein.kr/blog/archives/4389 )  
- [(일어) Linux Observability with BPF Performance Tools](https://speakerdeck.com/govargo/cndt2020-linux-observability-with-bpf-performance-tools ) 
- [(일어) eXpress Data Path(XDP) 개요와 LINE에서의 이용 사례](https://speakerdeck.com/yunazuno/brief-summary-of-xdp-and-use-case-at-line )  
- [(일어) 분산 애플리케이션의 의존 발견에 알맞은 TCP/UDP 소켓에 기초로 저 부하 트레이싱](https://blog.yuuk.io/entry/2021/wsa08 )  
- [(일어) eBPF 컴파일러에 대응한 툴로 다양한 행동을 가시화하기](https://gihyo.jp/admin/serial/01/ubuntu-recipe/0688 )
- [(일어) sysfs 나 bpftool을 이용한 eBPF 활용](https://gihyo.jp/admin/serial/01/ubuntu-recipe/0692 )
- [(일어) libbpf 와 clang으로 포터블한 BPF CO-RE 바이너리 생성](https://gihyo.jp/admin/serial/01/ubuntu-recipe/0694 )
- [(일어) 입문 BPF CO-RE](https://gihyo.jp/admin/serial/01/ubuntu-recipe/0695 )
- [(일어) BCC로 eBPF 코드를 만들어 보았다](https://gihyo.jp/admin/serial/01/ubuntu-recipe/0690 )
- [(일어) eBPF으로 실현하는 컨테이너 런타임 보안](https://speakerdeck.com/tobachi/container-runtime-security-with-ebpf )  
- [(일어) Cilium Project에서 공개! eBPF를 이용하여 보안 관측성을 해주는 Tetragon](https://gihyo.jp/article/2022/08/kubernetes-cloudnative-topics-01 )
- [XDP를 활용한 네트워크 패킷 처리](https://www.monitorapp.com/ko/ebpf_xdp/ )
   
    
    
# lib
- [cilium](https://github.com/cilium/cilium )   eBPF-based Networking, Security, and Observability
    - [(일어) Go의 cilium/ebpf 으로 Xdpcap을 사용한다](https://terassyi.net/posts/2021/10/07/use-xdpcap.html )
- [Parca](https://news.hada.io/topic?id=6550 )  eBPF 기반의 지속적인 CPU/메모리 프로파일러      
- [loxilb-io/loxilb](https://github.com/loxilb-io/loxilb) loxilb is a cloud-native "edge" load-balancer stack built from grounds up using eBPF at its core
  
    
    
# ebpf-for-windows   
- [공식 저장소](https://github.com/microsoft/ebpf-for-windows )
- [(일어) Windows의 eBPF를 보안에 보호하기 위한 써움](https://blogs.trellix.jp/the-race-to-secure-ebpf-for-windows )   





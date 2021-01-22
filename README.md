<a href="https://play.google.com/store/apps/details?id=com.grinplusplus.mobile"><img height="100" src="https://raw.githubusercontent.com/GrinPlusPlus/GrinPlusPlusMobile/master/google-play-badge.png"/></a>&nbsp;&nbsp;&nbsp;
<a href="https://keybase.pub/dtavarez/grin%2B%2B/apk/"><img height="100" src="https://level01.io/wp-content/uploads/2020/10/direct-download-apk-300x116.png"/></a>

# Grin++ (Android)

Grin++ is an easy to use Grin Wallet. With Grin++ you can create multiple accounts separately. A crypto wallet works just like a bank account where we can store the transactions made, can use to send and receive digital currency.

![001|240x500, 75%](https://aws1.discourse-cdn.com/standard10/uploads/grin/optimized/2X/7/70680072a2d5a6243fca3eaf2c248bff0dd65936_2_180x375.jpeg) ![002|240x500, 75%](https://aws1.discourse-cdn.com/standard10/uploads/grin/optimized/2X/9/94ef31b2fb235fc7682175ff3a0f941e760fb564_2_180x375.jpeg) ![003|240x500, 75%](https://aws1.discourse-cdn.com/standard10/uploads/grin/optimized/2X/0/0bd081b647f4e15a597e8cb9fa5ca5d9d01cd515_2_180x375.jpeg) ![003|240x500, 75%](https://aws1.discourse-cdn.com/standard10/uploads/grin/optimized/2X/0/036ab309313291e61eb37f28074aefc1879d7849_2_180x375.jpeg) 

### Features

- Multiple grin wallets.
- Backup/Restore wallets via seed phrase.
- Sending and Receiving $grin via Tor. 
- Sending and Receiving $grin Slatepack Messages.
- Address availability checker
- A Full Grin node runs as a Service.

### Requirments

* Minimum version required: **Android 9.0 (API Level 28)**.
* Architecture supported: **64bit**.
* ARM: **AArch64** or **ARM64**.
* Minimum RAM recommended: **4GB**.
* Programming Language: **C#**.
* Framework: **Xamarin.Forms**.

## Building APK

- Clone this repository.
- Open the Solution using Visual Studio or Xamarin Studio.
- Build

## Building Node

```
cd /home/david/Projects/
git clone https://github.com/GrinPlusPlus/GrinPlusPlus
cd GrinPlusPlus
git checkout android-arm64

vcpkg install --overlay-triplets=vcpkg/custom_triplets --triplet arm64-android-static --debug mio && \
vcpkg install --overlay-triplets=vcpkg/custom_triplets --triplet arm64-android-static --debug libuuid && \
vcpkg install --overlay-triplets=vcpkg/custom_triplets --triplet arm64-android-static --debug fmt && \
vcpkg install --overlay-triplets=vcpkg/custom_triplets --triplet arm64-android-static --debug asio && \
vcpkg install --overlay-triplets=vcpkg/custom_triplets --triplet arm64-android-static --debug zlib && \
vcpkg install --overlay-triplets=vcpkg/custom_triplets --overlay-ports=vcpkg/custom_ports --triplet arm64-android-static --debug minizip && \
vcpkg install --overlay-triplets=vcpkg/custom_triplets --overlay-ports=vcpkg/custom_ports --triplet arm64-android-static --debug secp256k1-zkp && \
vcpkg install --overlay-triplets=vcpkg/custom_triplets --overlay-ports=vcpkg/custom_ports --triplet arm64-android-static --debug rocksdb && \
vcpkg install --overlay-triplets=vcpkg/custom_triplets --overlay-ports=vcpkg/custom_ports --triplet arm64-android-static --debug civetweb && \
vcpkg install --overlay-triplets=vcpkg/custom_triplets --overlay-ports=vcpkg/custom_ports --triplet arm64-android-static --debug roaring && \
vcpkg install --overlay-triplets=vcpkg/custom_triplets --overlay-ports=vcpkg/custom_ports --triplet arm64-android-static --debug libsodium

export NDK=/home/david/Android/Sdk/ndk/canary
export API=29
export TOOLCHAIN=$NDK/toolchains/llvm/prebuilt/linux-x86_64
export TARGET=aarch64-linux-android
export AR=$TOOLCHAIN/bin/$TARGET-ar
export AS=$TOOLCHAIN/bin/$TARGET-as
export CC=$TOOLCHAIN/bin/$TARGET$API-clang
export CXX=$TOOLCHAIN/bin/$TARGET$API-clang++
export LD=$TOOLCHAIN/bin/$TARGET-ld
export RANLIB=$TOOLCHAIN/bin/$TARGET-ranlib
export STRIP=$TOOLCHAIN/bin/$TARGET-strip

rm -Rf /home/david/Projects/GrinPlusPlus/build && mkdir /home/david/Projects/GrinPlusPlus/build && \
cmake -S /home/david/Projects/GrinPlusPlus -B /home/david/Projects/GrinPlusPlus/build -G Ninja \
      -D CMAKE_BUILD_TYPE=Release -D GRINPP_TESTS=OFF -D GRINPP_TOOLS=OFF \
      -D CMAKE_MAKE_PROGRAM=/home/david/Tools/vcpkg/downloads/tools/ninja-1.10.0-linux/ninja \
      -D CMAKE_TOOLCHAIN_FILE=/home/david/Tools/vcpkg/scripts/buildsystems/vcpkg.cmake -DVCPKG_TARGET_TRIPLET=arm64-android-static
cmake --build /home/david/Projects/GrinPlusPlus/build
```

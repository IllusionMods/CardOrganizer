# General package information
pkgname=cardorganizer-git
pkgver=1.0.0
pkgrel=2
pkgdesc="Script to organize Illusion game cards"
url="https://github.com/Keelhauled/CardOrganizer"
license=("GPL3")
arch=("any")

_tempreponame="CardOrganizer-repo"
_binname="cardorganizer"

# Dependencies
depends=("python" "python-pyahocorasick" "python-tqdm")

# Download information
source=("${_tempreponame}::git+file://${PWD}")
#source=("${_tempreponame}::git+${url}")
sha256sums=("SKIP")

package() {
  cd "${srcdir}/${_tempreponame}"
  install -D -m755 "${_binname}" "${pkgdir}/usr/bin/${_binname}"
}

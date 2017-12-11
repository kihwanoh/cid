FROM syncromatics/build-box:release-5.4.1-android-support

ARG is_pull_request=
ARG travis_tag=
ARG api_key=

ENV TRAVIS_PULL_REQUEST=$is_pull_request
ENV TRAVIS_TAG=$travis_tag
ENV API_KEY=$api_key

WORKDIR /build

COPY . .

RUN ./build.sh

FROM nginx:alpine as base
RUN apk update && apk add jq

FROM node:lts as restore
WORKDIR /app
COPY src/Web/Holefeeder.Web/package.json ./
COPY src/Web/Holefeeder.Web/yarn.lock ./
RUN yarn

FROM restore as build
ARG BUILD_VERSION
ARG VERSION=${BUILD_VERSION:-99.99.99}
ENV NODE_OPTIONS="--max_old_space_size=8096"
WORKDIR /app
COPY src/Web/Holefeeder.Web/. .
RUN node_modules/.bin/ng build
RUN echo $VERSION > dist/holefeeder-web/assets/version.txt

FROM base as final
COPY --from=build /app/dist/holefeeder-web /usr/share/nginx/html
COPY src/Web/Holefeeder.Web/99-load-envvars.sh /docker-entrypoint.d
RUN chmod +x /docker-entrypoint.d/99-load-envvars.sh

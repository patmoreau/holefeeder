FROM nginx:alpine as base
RUN apk update && apk add jq

FROM node as restore
WORKDIR /app
COPY src/Web/Holefeeder.Web/package*.json .
RUN npm install

FROM restore as build
ENV NODE_OPTIONS="--max_old_space_size=8096"
WORKDIR /app
COPY src/Web/Holefeeder.Web/. .
RUN npm run build -- --prod

FROM base as final
COPY --from=build /app/dist /usr/share/nginx/html
COPY src/Web/Holefeeder.Web/99-load-envvars.sh /docker-entrypoint.d
RUN chmod +x /docker-entrypoint.d/99-load-envvars.sh

##### Stage 1
FROM node:latest as node
LABEL author="Drifter Apps Inc."
WORKDIR /app
COPY package.json package.json
COPY package-lock.json package-lock.json
RUN npm install
COPY . .
RUN npm run build -- --prod

##### Stage 2
FROM nginx:alpine
VOLUME /var/cache/nginx
COPY --from=node /app/dist /usr/share/nginx/html
COPY ./config/nginx.conf /etc/nginx/conf.d/default.conf

# docker build -t holefeeder-web -f nginx.prod.dockerfile .
# docker run -d -p 8080:80 holefeeder-web
# docker tag holefeeder-web:latest registry.gitlab.com/drifterapps/holefeeder-web:1.0.0
# docker push registry.gitlab.com/drifterapps/holefeeder-web
# docker login YN6hzx73GnzmDzTYfnnq
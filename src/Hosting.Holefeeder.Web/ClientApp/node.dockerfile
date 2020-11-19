FROM node:alpine
LABEL author="Drifter Apps Inc."

ENV NODE_ENV production
WORKDIR /usr/src/app

COPY ["package.json", "package-lock.json*", "npm-shrinkwrap.json*", "./"]
RUN npm install --production --silent && mv node_modules ../

COPY . .

EXPOSE 4200

CMD npm start
FROM node:22.20.0-slim AS build
WORKDIR /app
COPY . ./
RUN npm ci && npm run build

# production environment
# FROM nginxinc/nginx-unprivileged:1.29.1-alpine-slim
FROM nginx:alpine-slim
COPY --from=build /app/build /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 3000
CMD ["nginx", "-g", "daemon off;"]

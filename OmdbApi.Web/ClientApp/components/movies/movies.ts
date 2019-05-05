import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import axios from 'axios';
import { APIService } from '../../APIService';
const apiService = new APIService();

interface movie {
    title: string;
    year: string;
    rated: string;
    actors: string;
    awards: string;
    ratings: ratings[];
    error: string;
    response: boolean;
    poster: string;
}

interface ratings {
    id: number;
    source: string;
    value: string;
    movieId: number;
}

@Component
export default class MoviesComponent extends Vue {
    // token will get from login method
    token: string = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJtdXN0YWZhMiIsImp0aSI6IjEwZTE3ZmJhLTUyNjItNDZlYS1iNWJkLWMzZTFiMGEzZWVhZSIsImVtYWlsIjoibXVzdGFmYTJAZ21haWwuY29tIiwiVXNlcklkIjoiNCIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoibXVzdGFmYTIiLCJleHAiOjE1NTcwNjI2MDEsImlzcyI6Imh0dHBzOi8vZ2l0aHViLmNvbS9tdXN0YWZhYWxrYW42NC9PbWRiQXBpQ29yZVByb2plY3QiLCJhdWQiOiJodHRwczovL2dpdGh1Yi5jb20vbXVzdGFmYWFsa2FuNjQvT21kYkFwaUNvcmVQcm9qZWN0In0.vh0GurWmeMK9o82zzeevt6x5Ksdqpv-zc_UFG7iP71w";
    movie: movie = <movie>{
        title: "",
        year: "",
        rated: "",
        actors: "",
        awards: "",
        ratings: [],
        response: false,
        poster : "",
        error: ""
    };

    getMovie() {
        var config = {
            headers: {
                'Authorization': "bearer " + this.token,
                'Content-Type': 'application/json'
            }
        };

        var bodyParameters = {
            title: "avengers"
        }
        axios({
            method: 'get',
            url: apiService.API_URL + '/api/Movie/SearchMovie?title=avengers',
            headers: {
                Authorization: 'Bearer ' + this.token,
                'Content-Type': 'application/json'
            }
            }).then((response: any) => {
                console.log(response.data);
                this.movie = response.data;
            })
            .catch((error: any) => {
                console.log(error);
                if (error.response && error.response.status === 401) {
                    window.location.href = "logon";
                } else {
                    // Handle error however you want
                }
            });
    }

    mounted() {
        this.getMovie();
    }
}
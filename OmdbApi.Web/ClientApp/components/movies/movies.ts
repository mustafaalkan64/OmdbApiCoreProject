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

interface movieCollection {
    search: movie[];
    totalResult: number;
    response: boolean;
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
    token: string = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJtdXN0YWZhMiIsImp0aSI6IjQzMDZmNDNjLTdlZjYtNDNmZC1iOTk5LTkwMGUxYTY1MDhmMyIsImVtYWlsIjoibXVzdGFmYTJAZ21haWwuY29tIiwiVXNlcklkIjoiNCIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoibXVzdGFmYTIiLCJleHAiOjE1NTc2ODA5NDUsImlzcyI6Imh0dHBzOi8vZ2l0aHViLmNvbS9tdXN0YWZhYWxrYW42NC9PbWRiQXBpQ29yZVByb2plY3QiLCJhdWQiOiJodHRwczovL2dpdGh1Yi5jb20vbXVzdGFmYWFsa2FuNjQvT21kYkFwaUNvcmVQcm9qZWN0In0.e3qdaddwD55-1eNO9nTribpfwq2iQu3onHN4e_2wMnk";
    movie: movie = <movie>{
        title: "",
        year: "",
        rated: "",
        actors: "",
        awards: "",
        ratings: [],
        response: false,
        poster: "",
        error: ""
    };
    wholeResponse: movieCollection = <movieCollection>{
        search: [],
        response: true,
        totalResult: 10
    };
    loading: boolean = false;
    term: string = "";
    noData: boolean = false;

    searchMovie() {
        return this.getMovie();
    }

    data() {
        return {
            wholeResponse: [],
            loading: false,
            noData: true
        }
    }

    getMovie() {
        console.log(this.term);
        if (this.term.length < 3)
            console.log("En az 3 Karakter");
        else {
            this.loading = true;
            axios({
                method: 'get',
                url: apiService.API_URL + '/api/Movie/SearchMovie?term=' + this.term,
                headers: {
                    Authorization: 'Bearer ' + this.token,
                    'Content-Type': 'application/json'
                }
            }).then((response: any) => {
                debugger;
                if (response.data.response == true) {
                    this.wholeResponse = response.data.search;
                    this.loading = false;
                    this.noData = false;
                }
                else {
                    this.noData = true;
                    this.loading = false;
                }
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

    }

    mounted() {
        this.getMovie();
    }
}
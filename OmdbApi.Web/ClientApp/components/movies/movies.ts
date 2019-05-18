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
    token: string = localStorage.getItem('token') || '';
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
    $router: any;

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
                this.wholeResponse = response.data.search;
                if (response.data.response == true) {
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
                    this.$router.push('logon');;
                } else {
                    // Handle error however you want
                }
            });
        }

    }

    //mounted() {
    //    this.getMovie();
    //}
}